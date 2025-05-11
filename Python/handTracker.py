import cv2 as cv
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector


class handTracker:
    def __init__(self, camera=cv.VideoCapture(0), xyMax=(1920, 1080)):
        self.handDetector = HandDetector(maxHands=1, detectionCon=0.8)
        self.camera = camera
        self.yMax = xyMax[1]

    def get_hand_position(self):
        success, image = self.camera.read()
        if not success:
            return None, None

        hands, image = self.handDetector.findHands(image, flipType=True)

        hand_position = []  # 빈 리스트로 초기화하여 안전하게 반환
        if hands:
            lmList = hands[0]['lmList']
            for x, y, z in lmList:
                hand_position.extend([x, self.yMax - y, z])

        return hand_position, image
