import socket
import numpy as np
import errno

MESSAGE_TYPE = ["SCREEN", "CAPTURE"]

def get_send_str(iris, hands):
    iris_str = f"{iris[0]},{iris[1]}"
    hands_str = "/".join(f"{h[0]},{h[1]},{h[2]}" for h in hands)
    send_str = iris_str + "##" + hands_str
    return send_str

class UDPState:
    """
    UDP의 상태 클래스
    각 상태 별 필요한 메소드를 포함합니다.
    """
    def __init__(self):
        self.current = 0

        self.calibration_points = {
            "LeftTop": [-1, -1],
            "RightTop": [-1, -1],
            "LeftBottom": [-1, -1],
            "RightBottom": [-1, -1]
        }

        self.screen_size = [-1, -1]
        self.pre_message = "0"

    def is_get_screen(self):
        return self.screen_size == (-1, -1)

    def is_all_captured(self):
        return self.calibration_points["RightBottom"][0] != -1

    def capture(self, w, h, iris):
        x, y = iris[0], iris[1]
        self.calibration_points = {
            "LeftTop": [x - w, y - h],
            "RightTop": [x + w, y - h],
            "LeftBottom": [x - w, y + h],
            "RightBottom": [x + w, y + h]
        }

class UDPManager:
    """
    UDP 관리 클래스

    이 클래스는 Unity 애플리케이션과 Python 코드 간의 UDP 통신을 담당합니다.
    모든 메시지는 UTF-8 인코딩된 문자열 형식으로 주고받습니다.
    """
    def __init__(self, ip="127.0.0.1", send_port=5000, receive_port=5001):
        """
        UDP 관리자 초기화

        Args:
            ip (str): 통신할 IP 주소 (기본값: "127.0.0.1" - localhost)
            send_port (int): 데이터 송신용 포트 번호 (기본값: 5000)
            receive_port (int): 데이터 수신용 포트 번호 (기본값: 5001)
        """
        self.ip = ip  # 통신 대상 IP 주소
        self.send_port = send_port  # 송신 포트
        self.receive_port = receive_port  # 수신 포트
        
        # 송신용 소켓 초기화
        self.send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP 소켓 생성
        
        # 수신용 소켓 초기화
        self.receive_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP 소켓 생성
        self.receive_sock.bind((self.ip, self.receive_port))  # 수신 포트에 바인딩
        self.receive_sock.setblocking(False)  # 블로킹 모드 설정 (비동기 통신)
        self.running = False
        
    def send(self, data):
        """
        데이터를 유니티로 전송

        Args:
            data (str): 전송할 데이터 문자열

        Returns:
            bool: 성공 시 True, 실패 시 False

        Notes:
            - 데이터는 UTF-8로 인코딩 후 전송됨
            - 권장 전송 형식은 문자열(str)
        """
        try:
            # 문자열을 UTF-8로 인코딩하여 바이트로 변환 후 송신
            self.send_sock.sendto(data.encode('utf-8'), (self.ip, self.send_port))
        except Exception as e:
            print(f"UDP 전송 오류: {e}")  # 오류 메시지 출력
            return False
        return True  # 전송 성공

    def receive(self, message_type=None):
        """
        비동기 수신

        Args:
            message_type (str): 처리할 메시지 유형

        Returns:
            str: message, 없으면 ""

        Notes:
            - 수신 데이터는 UTF-8로 디코딩하여 문자열로 변환
        """
        try:
            # 데이터 수신 (최대 256바이트)
            data, addr = self.receive_sock.recvfrom(256)
            message = data.decode('utf-8')

            return message  # 메시지 수신 성공
        except OSError as e:
            # 자원이 일시적으로 사용 불가능함 오류 처리
            if e.errno == errno.EAGAIN or e.errno == errno.EWOULDBLOCK:
                return ""  # 메시지 수신 실패 (정상)
            else:
                print(f"UDP 수신 오류: {e}")  # 진짜 오류만 출력
                return ""
        except Exception as e:
            print(f"UDP 수신 오류: {e}")  # 오류 메시지 출력
            return ""


    def close(self):
        """
        소켓 연결 종료
        
        모든 소켓을 닫고 연결 종료. 프로그램 종료 전 반드시 호출해야 함.
        """
        self.send_sock.close()  # 송신 소켓 닫기
        self.receive_sock.close()  # 수신 소켓 닫기
