import cv2 as cv
from handTracker import handTracker

camera = cv.VideoCapture(0)
hand = handTracker(camera, (1920, 1080))

while True:
    hand_position, image = hand.get_hand_position()

    if image is not None:
        cv.imshow('Hand Tracker Test', image)
        if cv.waitKey(1) & 0xFF == 27:
            break
    else:
        print("카메라에서 이미지를 얻지 못했습니다.")

camera.release()
cv.destroyAllWindows()
