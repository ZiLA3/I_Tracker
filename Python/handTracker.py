import cv2 as cv
import mediapipe as mp
from cvzone.HandTrackingModule import HandDetector

class handTracker:
    def __init__(self, camera=cv.VideoCapture(0)):
        self.handDetector = HandDetector(maxHands=1, detectionCon=0.8)
        self.camera = camera

    def get_hand_position(self):
        """
        현재 손 추적 반환
        y값 보정 필요! y = (height - y)
        """
        success, image = self.camera.read()
        if not success:
            return None, None
        hands, image = self.handDetector.findHands(image, flipType = True) # 좌우 반전
        
        if hands:
            lmList = hands[0]['lmList']
            hand_position = []
            for x, y, z in lmList:
                hand_position.extend([x, y, z])
        
        return hand_position, image # y 보정 필요