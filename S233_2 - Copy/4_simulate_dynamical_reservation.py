from heapq import heappop, heappush
from numpy.random import default_rng

rng = default_rng()

# unit: second
def generate_interval():
    return rng.exponential(scale=1.35)


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


SIM_DURATION = 10 * 3600  # unit: second
HANDOVER_RESERVED = 5


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
        skip = HANDOVER_RESERVED
        for channel in range(10):
            if channel in self.channels[cell]:
                if self.channels[cell][channel]['handover']:
                    skip -= 1
                continue
            else:
                if skip <= 0:
                    return channel
                else:
                    skip -= 1
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
        after_duration = (2 - location % 2) / speed

        if after_duration <= duration:
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
            self.push_event(after_duration, end)
        else:
            handover = {
                "type": "handover",
                "current_channel": channel,
                "next_cell": next_cell,
                "duration": duration - after_duration,
                "speed": speed,
            }
            self.push_event(after_duration, handover)

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


system = System()
while not system.is_time_up(SIM_DURATION):
    system.deliver_next_event()

total_call = system.successful_call + system.blocked_call + system.dropped_call
print(
    f"total: {total_call} blocked: {system.blocked_call} dropped: {system.dropped_call}"
)
print(f"blocked call: {system.blocked_call / total_call * 100:.2f}%")
print(f"dropped call: {system.dropped_call / total_call * 100:.2f}%")

# Output
# 1 channel
# total: 26416 blocked: 4197 dropped: 3862
# blocked call: 15.89%
# dropped call: 14.62%
# 2 channel
# total: 26633 blocked: 4877 dropped: 3679
# blocked call: 18.31%
# dropped call: 13.81%
# 3 channel
# total: 26673 blocked: 5852 dropped: 3553
# blocked call: 21.94%
# dropped call: 13.32%
# 4 channel
# total: 26573 blocked: 6962 dropped: 3021
# blocked call: 26.20%
# dropped call: 11.37%
# 5 channel
# total: 26385 blocked: 8611 dropped: 2540
# blocked call: 32.64%
# dropped call: 9.63%