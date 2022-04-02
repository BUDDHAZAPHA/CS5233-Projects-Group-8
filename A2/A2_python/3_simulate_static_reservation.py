from heapq import heappop, heappush
from numpy.random import default_rng
import numpy as np 
import statistics as stat
from scipy.stats import t

# SCALED_UP = 1 indicates the base level network traffic
# SCALED_UP < 1 indicates ((1-SCALED_UP)*100)% increased network traffic
SCALED_UP = 1
rng = default_rng()

# unit: second
def generate_interval():
    return rng.exponential(scale=1.35*SCALED_UP)


# call arrival location relative to entrance of highway, i.e. 0KM of cell 0
# unit: km
def generate_location():
    return rng.uniform(low=0, high=40)


# unit: second
def generate_duration():
    return rng.exponential(scale=120)


# unit: km/h
def generate_speed():
    return rng.normal(loc=90, scale=8.22)


SIM_DURATION = 100 * 3600  # unit: second
HANDOVER_RESERVED = 1


class System:
    def __init__(self):
        # list of channel id => call mapping, channel id in range [0, 20)
        self.channels = [{} for _ in range(20)]
        # priority queue of event, i.e. (instant, event id, dict) tuple
        self.event_queue = []
        self.event_id = 0
        # unit: second
        self.now = 0

        self.blocked_call = 0
        self.dropped_call = 0
        self.successful_call = 0

        self.generate_call()

    # everything need to be changed for problem 2 & 3: the two handle methods
    # return channel id to allocate the channel to the call
    # return None to block/drop the call

    def handle_initiate(self, cell):
        for channel in range(10 - HANDOVER_RESERVED):
            if channel not in self.channels[cell]:
                return channel
        return None

    def handle_handover(self, cell):
        for channel in range(10):
            if channel not in self.channels[cell]:
                return channel
        return None

    def push_event(self, after_duration, event):
        self.event_id += 1
        heappush(self.event_queue, (self.now + after_duration, self.event_id, event))
        return self.event_id

    def generate_call(self):
        interval = generate_interval()  # second
        location = generate_location()  # km
        duration = generate_duration()  # second
        speed = generate_speed() / 3600  # km/second

        initiate = {
            "type": "initiate",
            "location": location,
            "duration": duration,
            "speed": speed,
        }
        self.push_event(interval, initiate)

    def on_initiate(self, location, duration, speed):
        self.generate_call()

        cell = int(location // 2)
        assert 0 <= cell < 20

        channel = self.handle_initiate(cell)
        if channel is None:
            self.blocked_call += 1
            return

        assert 0 <= channel < 10
        assert channel not in self.channels[cell]
        self.channels[cell][channel] = {
            "since": self.now,
            "handover": False,
            # anything else useful?
        }

        self.push_next_event_for_call(cell, channel, location, duration, speed)

    def push_next_event_for_call(self, cell, channel, location, duration, speed):
        next_cell = cell + 1
        # distance in current cell / speed
        cell_duration = (2 - location % 2) / speed

        if cell_duration >= duration:
            end = {
                "type": "end",
                "cell": cell,
                "channel": channel,
            }
            self.push_event(duration, end)
        elif next_cell == 20:
            end = {
                "type": "end",
                "cell": cell,
                "channel": channel,
            }
            self.push_event(cell_duration, end)
        else:
            handover = {
                "type": "handover",
                "current_channel": channel,
                "next_cell": next_cell,
                "duration": duration - cell_duration,
                "speed": speed,
            }
            self.push_event(cell_duration, handover)

    def on_handover(self, current_channel, next_cell, duration, speed):
        del self.channels[next_cell - 1][current_channel]

        channel = self.handle_handover(next_cell)
        if channel is None:
            self.dropped_call += 1
            return

        assert 0 <= channel < 10
        assert channel not in self.channels[next_cell]
        self.channels[next_cell][channel] = {
            "since": self.now,
            "handover": True,
            #
        }

        location = next_cell * 2  # entry location of the cell
        self.push_next_event_for_call(next_cell, channel, location, duration, speed)

    def on_end(self, cell, channel):
        del self.channels[cell][channel]
        self.successful_call += 1

    def deliver_next_event(self):
        self.now, _, event = heappop(self.event_queue)
        # not make this too fancy :)
        event_type = event["type"]
        del event["type"]
        if event_type == "initiate":
            self.on_initiate(**event)
        elif event_type == "handover":
            self.on_handover(**event)
        elif event_type == "end":
            self.on_end(**event)
        else:
            assert False

    def is_time_up(self, time_limit):
        return self.event_queue[0][0] >= time_limit

REPLICATIONS = 20
CONFLEVEL = 99
two_tail = (1-CONFLEVEL/100)/2
blocked_samples = []
dropped_samples = []
for r in range(REPLICATIONS):
    system = System()
    while not system.is_time_up(SIM_DURATION):
        system.deliver_next_event()

    total_call = system.successful_call + system.blocked_call + system.dropped_call
    print(
        f"total: {total_call} blocked: {system.blocked_call} dropped: {system.dropped_call}"
    )
    blocked_samples.append(system.blocked_call / total_call)
    dropped_samples.append(system.dropped_call / total_call)

mena_block_rate = np.mean(blocked_samples) * 100
block_conf_inter = 100 * t.ppf(1-two_tail,REPLICATIONS-1) * stat.pstdev(blocked_samples) / np.sqrt(REPLICATIONS)
mean_drop_rate = np.mean(dropped_samples) * 100
drop_conf_inter = 100 * t.ppf(1-two_tail,REPLICATIONS-1) * stat.pstdev(dropped_samples) / np.sqrt(REPLICATIONS)
print(f"blocked call: {mena_block_rate:.2f}% +/- {block_conf_inter:.4f}%")
print(f"dropped call: {mean_drop_rate:.2f}% +/- {drop_conf_inter:.4f}%")

'''
Sample Output:
total: 280440 blocked: 5549 dropped: 2121
total: 281417 blocked: 5629 dropped: 2174
total: 280962 blocked: 5707 dropped: 2112
total: 280790 blocked: 5724 dropped: 2206
total: 281097 blocked: 5811 dropped: 2247
total: 280894 blocked: 5650 dropped: 2063
total: 280986 blocked: 5470 dropped: 2172
total: 280264 blocked: 5569 dropped: 2138
total: 280657 blocked: 5524 dropped: 2113
total: 281569 blocked: 5712 dropped: 2242
total: 280670 blocked: 5613 dropped: 2074
total: 279180 blocked: 5714 dropped: 1998
total: 280566 blocked: 5488 dropped: 2047
total: 280587 blocked: 5893 dropped: 2165
total: 280169 blocked: 5473 dropped: 2106
total: 280943 blocked: 5585 dropped: 1976
total: 280556 blocked: 5652 dropped: 2159
total: 279984 blocked: 5442 dropped: 2159
total: 280330 blocked: 5432 dropped: 2144
total: 280332 blocked: 5590 dropped: 2263
blocked call: 2.00% +/- 0.0272%
dropped call: 0.76% +/- 0.0168%
'''