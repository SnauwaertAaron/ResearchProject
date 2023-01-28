import time, os, json, random, asyncio
from azure.iot.device import IoTHubDeviceClient
from Cryptodome.Cipher import AES
from Cryptodome.Protocol.KDF import PBKDF2
from Cryptodome.Util.Padding import unpad

# Function to read from file
def read_file(path):
    file = open(path,"r")
    content = file.read()
    file.close()
    return content

# Function to decrypt binary data from file
def decrypt_file(file):
    #defie salt
    salt = b'\x09\x02\x00\x00\x08\x07\x03\x01'
    # Read Binary file
    with open(file, 'rb') as f:
            encrypted_data = f.read()
    
    # Derive the key and IV from the password and salt
    kdf = PBKDF2(required, salt, dkLen=48, count=1000)
    key, iv = kdf[:32], kdf[32:]

    # Decrypt the data using the AES algorithm
    cipher = AES.new(key, AES.MODE_CBC, iv)
    content = cipher.decrypt(encrypted_data)
    # Delete pading
    content = unpad(content, AES.block_size)
    # Decode to utf8
    content = str(content, 'utf-8')
    return content

# Extract needed variables from files
required = read_file("D:\School\SEM7\ResearchProject\ResearchProjectSimulation\Assets\Resources\EK.txt")
conn_str = decrypt_file("D:\School\SEM7\ResearchProject\IoTHub_conn.bin")

# Create instance of the device client using the authentication provider
device_client = IoTHubDeviceClient.create_from_connection_string(conn_str)

# Connect the device client.
device_client.connect()

previous_payload = 0

while True:
    try:
        # Decrypt sensordata
        payload = decrypt_file('D:\School\SEM7\ResearchProject\ResearchProjectSimulation\Assets\Resources\SensorData.bin')
        #Check if payload is the same -> else skip payload
        if payload != previous_payload:
            # Send a single message
            print(f"Sending: {payload} to IoT Hub.")
            device_client.send_message(payload)
            print("Message successfully sent!\n")
        
            # updqte previous pqyloqd
            previous_payload = payload

            time.sleep(1)

    except:
            # if fails -> disconnect
            device_client.disconnect()