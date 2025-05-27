import cv2 as cv
import callbacks
from Tracker import Tracker
from xyMapper import xyMapper
from UDPManager import UDPManager

VIDEO_INDEX = 0

IP = "127.0.0.1"
SEND_PORT = 5000
RECEIVE_PORT = 5001

#TODO: UDPManager 에 내장 가능 할 듯
CalibrationPoints = {
    'left_top': (-1, -1),
    'right_top': (-1, -1),
    'left_bottom': (-1, -1),
    'right_bottom': (-1, -1)
}
ScreenSize = (-1, -1)

class App:
    def __init__(self):
        self.video = cv.VideoCapture(VIDEO_INDEX)
        self.tracker = Tracker()
        self.udp = UDPManager(IP, SEND_PORT, RECEIVE_PORT)

    def read_video(self):
        success, image = self.video.read()
        if not success:
            print("!Error: VideoCapture read failed.")
            exit()
        return image

    def process(self):
        image = self.read_video()

        iris, hands = self.tracker.process(image, True)

        # TODO: UDP Manager
        """
        UDPManager에 State별 process 함수 있으면 편할 것 같음
        """

        cv.imshow("Tracker", image)
        if cv.waitKey() & 0xFF == ord('q'):
            return False
        return True

    def terminate():
        cv.destroyAllWindows()

if __name__ == "__main__":
    app = App()
    while app.process():
        continue
    app.terminate()