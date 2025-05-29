import cv2 as cv
import numpy as np
import callbacks
from Tracker import Tracker
from xyMapper import xyMapper
from UDPManager_ import UDPManager

class UDPState:
    def __init__(self):
        self.current = 0

        self.calibration_points = {
            "LeftTop": [-1, -1],
            "RightTop": [-1, -1],
            "LeftBottom": [-1, -1],
            "RightBottom": [-1, -1]
        }
        self.message_to_points = {
            "1": "LeftTop",
            "2": "RightTop",
            "3": "LeftBottom",
            "4": "RightBottom"
        }

        self.temp_array = np.empty((2, 0))
        self.screen_size = [-1, -1]
        self.pre_message = "0"

    def is_get_screen(self):
        return self.screen_size == (-1, -1)

    def is_all_captured(self):
        return self.calibration_points["RightBottom"][0] != -1

    def capture(self, iris):
        iris = np.array(iris).reshape((2, 1))
        self.temp_array = np.hstack([self.temp_array, iris])

    def end_capture(self, msg):
        x, y = self.temp_array[0].mean(), self.temp_array[1].mean()
        self.temp_array = np.empty((2, 0))

        self.calibration_points[self.message_to_points[msg]][0] = x
        self.calibration_points[self.message_to_points[msg]][1] = y


VIDEO_INDEX = 0

IP = "127.0.0.1"
SEND_PORT = 5000
RECEIVE_PORT = 5001

PROCESS_DELAY = 30

class App:
    def __init__(self):
        self.video = cv.VideoCapture(VIDEO_INDEX)

        self.tracker = Tracker()
        self.xyMapper = None
        self.udp = UDPManager(IP, SEND_PORT, RECEIVE_PORT)

        self.state = UDPState()
        self.debug_capture_int = 0

    def read_video(self):
        success, image = self.video.read()
        image = cv.flip(image, 1)
        if not success:
            print("!Error: VideoCapture read failed.")
            exit()
        return image

    def wait_process(self):
        screen_msg = "1920,1080" # self.udp.receive("screen_size")

        if screen_msg:
            x, y = screen_msg.split(',')
            self.state.screen_size = (int(x), int(y))
            self.state.current = 1

    def capture_process(self, iris):
        capture_msg = f"{self.debug_capture_int}" # self.udp.receive("captured")
        print(capture_msg)

        if capture_msg:
            if self.state.pre_message == capture_msg:
                self.state.capture(iris)
            else:
                self.state.end_capture(capture_msg)

            self.state.pre_message = capture_msg

        if self.state.is_all_captured():
            print("Capture End")
            self.xyMapper = xyMapper(self.state.calibration_points, self.state.screen_size)
            self.state.current = 2

    def run_process(self, iris, hands):
        #TODO:
        iris_mapping = self.xyMapper.map_coordinates(iris)
        iris_str = f"{iris_mapping[0]},{iris_mapping[1]}"
        hands_str = "/".join(f"{h[0]},{h[1]},{h[2]}" for h in hands)
        send_str = iris_str + "##" + hands_str
        print(send_str)
        self.udp.send(send_str)

    def process(self):
        image = self.read_video()
        iris, hands = self.tracker.process(image, False)
        match self.state.current:
            case 0:
                self.wait_process()
                pass
            case 1:
                self.capture_process(iris)
                pass
            case 2:
                self.run_process(iris, hands)
                pass

        cv.imshow("Tracker", image)

        key = cv.waitKey(PROCESS_DELAY)
        if key == ord('q'):
            return False
        elif key == ord('a'):
            self.debug_capture_int += 1

        return True

    def close(self):
        cv.destroyAllWindows()


if __name__ == "__main__":
    app = App()
    while app.process():
        continue
    app.close()