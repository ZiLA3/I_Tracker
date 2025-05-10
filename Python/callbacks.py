# 콜백 함수 모음

def on_screen_size(message):
    global screen_size
    screen_size = (message['width'], message['height'])
    print(f"화면 크기 수신: {screen_size}")
    return True

def on_captured(message):
    global calibration_points
    corner = message.get('corner')
    iris_pos = message.get('iris_pos')
    if corner and iris_pos:
        calibration_points[corner] = iris_pos
        print(f"{corner} 위치 캡처됨: {iris_pos}")
    return True

def on_pause():
    global paused
    paused = True
    print("일시정지됨")
    return True

def on_resume():
    global paused
    paused = False
    print("재개됨")
    return True

def on_kill():
    global running
    running = False
    print("종료 신호 수신")
    return False