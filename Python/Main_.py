import cv2 as cv
import Network
from Tracker import Tracker
from xyMapper import xyMapper

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
        self.udp = Network.UDPManager(IP, SEND_PORT, RECEIVE_PORT)
        self.state = Network.UDPState()

        self.debug_capture_int = 0

    def read_video(self):
        success, image = self.video.read()
        image = cv.flip(image, 1)
        if not success:
            print("!Error: VideoCapture read failed.")
            exit()
        return image

    def wait_process(self):
        print("Waiting Screen Size")
        screen_msg = self.udp.receive("screen_size") #"1920,1080"

        if screen_msg:
            print("Wait End")
            x, y = screen_msg.split(',')
            self.state.screen_size = (int(x), int(y))
            self.state.current = 1
            print("Capture Start...")

    def capture_process(self, iris):
        capture_msg = self.udp.receive("captured") #f"{self.debug_capture_int}"

        if capture_msg:
            if self.state.pre_message != capture_msg:
                print(f"capture count: {capture_msg}/4\n"
                      f"capture point: {iris}")
                self.state.capture(iris)
                self.state.end_capture(capture_msg)

            self.state.pre_message = capture_msg

        if self.state.is_all_captured():
            print("Capture End")
            self.xyMapper = xyMapper(self.state.calibration_points, self.state.screen_size)
            self.state.current = 2
            print("Running...")

    def run_process(self, iris, hands):
        iris_mapping = self.xyMapper.map_coordinates(iris)
        # print(f"{iris} -> {iris_mapping}")
        send_str = Network.get_send_str(iris_mapping, hands)
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
        self.udp.close()
        cv.destroyAllWindows()


if __name__ == "__main__":
    app = App()
    while app.process():
        continue
    app.close()