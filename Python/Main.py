import cv2 as cv
import Network
from Tracker import Tracker
from xyMapper import xyMapper

VIDEO_INDEX = 0

IP = "127.0.0.1"
SEND_PORT = 5000
RECEIVE_PORT = 5001

PROCESS_DELAY = 10

class App:
    """
    메인 클래스
    트래커에서 정보를 불러와 적절한 매핑 이후 다른 프로세스로 값을 넘겨줍니다. (UDP)
    """
    def __init__(self): 
        self.video = cv.VideoCapture(VIDEO_INDEX)

        self.tracker = Tracker()
        self.xyMapper = None
        self.udp = Network.UDPManager(IP, SEND_PORT, RECEIVE_PORT)
        self.state = Network.UDPState()

        print("Waiting Screen Size")

    def read_video(self):
        """
        카메라에서 이미지를 반환합니다.
        """
        success, image = self.video.read()
        image = cv.flip(image, 1)
        if not success:
            print("!Error: VideoCapture read failed.")
            exit()
        return image

    def wait_process(self):
        """
        state 1번: 화면 크기 정보를 입력받습니다.
        """
        screen_msg = "1920,1080" #self.udp.receive("screen_size") 

        if screen_msg:
            print("Wait End")
            x, y = screen_msg.split(',')
            self.state.screen_size = (int(x), int(y))
            self.state.current = 1
            print("Capture Start...")

    def capture_process(self, iris, shape):
        """
        state 2번: 모니터의 끝점 4개 좌표를 측정합니다.
        측정 이후 입력된 값을 기반으로 매퍼를 생성합니다.
        """
        capture_msg = self.udp.receive("captured")
        w, h, _ = shape
        w, h = w//4, h//4
        if capture_msg:
            print("Capture")
            self.state.capture(w, h, iris)
            self.state.pre_message = capture_msg

        if self.state.is_all_captured():
            print("Capture End")
            self.xyMapper = xyMapper(self.state.calibration_points, self.state.screen_size)
            self.state.current = 2
            print("Running...")

    def run_process(self, iris, hands):
        """
        state 3번: 매핑된 좌표들을 타 프로세스에 내보냅니다.
        """
        iris_mapping = self.xyMapper.map_coordinates(iris)
        # print(f"{iris} -> {iris_mapping}")
        send_str = Network.get_send_str(iris_mapping, hands)
        self.udp.send(send_str)

    def process(self):
        """
        현재 프로세스 상태에 따라 실행할 메소드를 결정합니다
        루프문 내에서 작동되어야 합니다.
        """
        image = self.read_video()
        iris, hands = self.tracker.process(image)

        match self.state.current:
            case 0:
                self.wait_process()
                pass
            case 1:
                self.capture_process(iris, image.shape)
                pass
            case 2:
                self.run_process(iris, hands)
                pass

        cv.imshow("Video", image)
        key = cv.waitKey(PROCESS_DELAY)
        if key == ord('q'):
            return False
        return True

    def close(self):
        self.udp.close()
        cv.destroyAllWindows()


if __name__ == "__main__":
    app = App()
    while app.process():
        continue
    app.close()
