import socket
import time

MESSAGE_TYPE = ["SCREEN", "CAPTURE"]



class UDPManager:
    def __init__(self, ip="127.0.0.1", send_port=5000, receive_port=5001):
        self.ip = ip  # 통신 대상 IP 주소
        self.send_port = send_port  # 송신 포트
        self.receive_port = receive_port  # 수신 포트
        
        # 송신용 소켓 초기화
        self.send_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP 소켓 생성
        
        # 수신용 소켓 초기화
        self.receive_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP 소켓 생성
        self.receive_sock.bind((self.ip, self.receive_port))  # 수신 포트에 바인딩
        self.receive_sock.setblocking(True)  # 블로킹 모드 설정 (동기 통신)
        # 비동기 처리시에는 False로 논블록킹 모드를 활성화해야합니다. (수신 데이터그램 대기)
        
    def send(self, data):
        try:
            # 문자열을 UTF-8로 인코딩하여 바이트로 변환 후 송신
            self.send_sock.sendto(data.encode('utf-8'), (self.ip, self.send_port))
        except Exception as e:
            print(f"UDP 전송 오류: {e}")  # 오류 메시지 출력
            return False
        return True  # 전송 성공

    def receive(self, message_type=None):
        try:
            # 데이터 수신 (최대 256바이트)
            data, addr = self.receive_sock.recvfrom(256)
            message = data.decode('utf-8')

            return message  # 메시지 수신 성공했지만 콜백 처리는 없음
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