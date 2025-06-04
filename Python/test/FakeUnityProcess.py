import socket
import threading

class UDPManager:
    def __init__(self, send_port, receive_port):
        self.send_port = send_port
        self.receive_port = receive_port
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.bind(('0.0.0.0', self.receive_port))
        self.running = False

    def send(self, message, ip='127.0.0.1'):
        self.sock.sendto(message.encode(), (ip, self.send_port))

    def receive(self):
        while self.running:
            data, addr = self.sock.recvfrom(1024)
            print(f"Received from {addr}: {data.decode()}")

    def start_receiving(self):
        self.running = True
        thread = threading.Thread(target=self.receive)
        thread.daemon = True
        thread.start()

    def stop_receiving(self):
        self.running = False


def cli_program():
    udp_manager = UDPManager(send_port=5001, receive_port=5000)
    udp_manager.start_receiving()

    print("Select mode:")
    print("1. Send screen size")
    print("2. Send numbers 0 to 4")
    print("3. Receive and print messages")

    while True:
        mode = input("Enter mode (1, 2, 3) or 'q' to quit: ")
        if mode == 'q':
            udp_manager.stop_receiving()
            break
        elif mode == '1':
            screen_size = "1920,1080"
            udp_manager.send(screen_size)
            print(f"Sent screen size: {screen_size}")
        elif mode == '2':
            num = input("Enter a number (0-4) to send: ")
            if num in ['0', '1', '2', '3', '4']:
                udp_manager.send(num)
                print(f"Sent number: {num}")
            else:
                print("Invalid number. Please enter 0, 1, 2, 3, or 4.")
        elif mode == '3':
            print("Receiving messages... Press 'q' to stop receiving.")
            # Receiving is already running in background
        else:
            print("Invalid mode. Please select 1, 2, or 3.")


if __name__ == '__main__':
    cli_program()
