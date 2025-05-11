import cv2 as cv
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector


class handTracker:
    def __init__(self, camera=cv.VideoCapture(0), xyMax=(1920, 1080)):
        self.handDetector = HandDetector(maxHands=1, detectionCon=0.8)
        self.camera = camera
        self.yMax = xyMax[1]

    def get_hand_position(self):
        """
        현재 손 추적 반환
        보정 후 나와서 그대로 데이터 보내면 됨 (DEPRECATED 내부 tracker.old 참조)
        hand값은 단순 좌표가 아니기 때문에 억지로 늘리면 안됨
        """
        success, image = self.camera.read()
        if not success:
            return None, None
        hands, image = self.handDetector.findHands(
            image, flipType=True)  # 좌우 반전

        if hands:
            lmList = hands[0]['lmList']
            hand_position = []
            for x, y, z in lmList:
                hand_position.extend([x, self.yMax - y, z])

        return hand_position, image
