import socket
import time


class UDPManager:
    """
    유니티와 통신하는 UDP 관리 클래스
    
    이 클래스는 Unity 애플리케이션과 Python 코드 간의 UDP 통신을 담당합니다.
    데이터 송신 및 수신 기능을 제공하며, 콜백 기반의 메시지 처리 시스템을 구현합니다.
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
        self.receive_sock.setblocking(True)  # 블로킹 모드 설정 (동기 통신)
        # 비동기 처리시에는 False로 논블록킹 모드를 활성화해야합니다. (수신 데이터그램 대기)
        
        # 이벤트 콜백 집합 (메시지 타입별 처리 함수)
        self.callbacks = {}  # 키: 메시지 타입, 값: 콜백 함수
        
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

    def receive(self, message_type=None, callback_args=None):
        """
        데이터를 수신하고 지정된 메시지 타입에 대한 콜백 함수 호출
        
        Args:
            message_type (str): 처리할 메시지 유형 (콜백 함수 선택용)
            callback_args: 콜백 함수에 추가로 전달할 인자 (선택적)
            
        Returns:
            bool: 성공 시 True, 실패 시 False
            
        Notes:
            - 수신 데이터는 UTF-8로 디코딩하여 문자열로 변환
            - 블로킹 모드에서는 데이터 수신 전까지 함수가 반환되지 않음
        """
        try:
            # 데이터 수신 (최대 256바이트)
            data, addr = self.receive_sock.recvfrom(256)
            # 바이트를 UTF-8로 디코딩하여 문자열로 변환
            message = data.decode('utf-8')
            
            # 등록된 콜백 실행 (message_type에 해당하는 콜백이 있는 경우)
            # 콜백 메소드 자체는 메시지(str)만 받고, 내부 로직은 전역 변수를 사용하여 설정
            if message_type in self.callbacks:
                # 콜백 함수 호출 (메시지와 추가 인자 전달)
                callback_result = self.callbacks[message_type](message, callback_args)
                return callback_result
                
            return True  # 메시지 수신 성공했지만 콜백 처리는 없음
        except Exception as e:
            print(f"UDP 수신 오류: {e}")  # 오류 메시지 출력
            return False
        
    
    def register_callback(self, message_type, callback):
        """
        특정 메시지 타입에 대한 콜백 함수 등록
        
        Args:
            message_type (str): 콜백을 등록할 메시지 유형
            callback (callable): 해당 메시지 타입 수신 시 호출할 콜백 함수
            
        Returns:
            bool: 등록 성공 시 True
            
        Notes:
            - 콜백 함수는 message, callback_args 형태의 인자를 받아야 함
        """
        self.callbacks[message_type] = callback  # 콜백 함수 등록
        return True

    def close(self):
        """
        소켓 연결 종료
        
        모든 소켓을 닫고 연결 종료. 프로그램 종료 전 반드시 호출해야 함.
        """
        self.send_sock.close()  # 송신 소켓 닫기
        self.receive_sock.close()  # 수신 소켓 닫기