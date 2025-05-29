import cv2 as cv
import mediapipe as mp


class HandTracker:
    """
    MediaPipe를 이용한 손 위치 추적 클래스
    """

    def __init__(self, camera=cv.VideoCapture(0), xyMax=(1920, 1080)):
        self.camera = camera
        self.yMax = xyMax[1]
        self.mp_hands = mp.solutions.hands
        self.hands = self.mp_hands.Hands(
            max_num_hands=1,
            min_detection_confidence=0.8,
            min_tracking_confidence=0.5
        )
        self.mp_draw = mp.solutions.drawing_utils

    def get_hand_position(self):
        success, image = self.camera.read()
        if not success:
            return None, None

        # BGR → RGB 변환 (MediaPipe는 RGB 입력 필요)
        image_rgb = cv.cvtColor(image, cv.COLOR_BGR2RGB)
        result = self.hands.process(image_rgb)

        hand_position = None

        if result.multi_hand_landmarks:
            landmarks = result.multi_hand_landmarks[0].landmark
            h, w, _ = image.shape
            hand_position = []
            for lm in landmarks:
                x = int(lm.x * w)
                y = int(lm.y * h)
                z = lm.z  # z는 정규화된 상대 깊이 값
                hand_position.extend([x, self.yMax - y, z])

            # 랜드마크 시각화
            self.mp_draw.draw_landmarks(image, result.multi_hand_landmarks[0], self.mp_hands.HAND_CONNECTIONS)
        print(hand_position)
        return hand_position, image

if __name__ == "__main__":
    tracker = HandTracker()
    while True:
        hand_pos, frame = tracker.get_hand_position()
        if frame is not None:
            cv.imshow("Hand Tracking", frame)
        if cv.waitKey(1) & 0xFF == ord('q'):
            break
    cv.destroyAllWindows()
