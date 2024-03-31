import asyncio
import json
import websockets
import tkinter as tk
import csv
import itertools
import time
import pyttsx3 

current_letter = ' '
last_blink_time = 0.0
word_buffer = ''
entry_index = 0

#Text to Speech Engine
text_to_speech_engine = pyttsx3.init()

#Authentication and Acces Request

async def connect_and_communicate():
    uri = "wss://localhost:6868"
    async with websockets.connect(uri) as websocket:
        print(f"Connected to {uri}")

        app_id = "com.mwahid2001.testt"
        client_id = "asyQcqXg679wQibHPw8MB73Sq7mu2wNkrHhSmiL4"
        client_secret = "eecVzRisx18r26HlNv5ZXbKgcaJqieBSkwzMHEx4NfnuDNA6YhelA5AHAmiNCeeIAxjwp7JD45Dopo6qmDwuof6ZFeBTAw9GYiJFLSaoSn4fV3NP5O4BUuLF9YDFDqWh"
        license_key = "137efe0d-f90c-46b5-8b36-66a4d477f891"
        
        # Request Access
        request_access_request = {
            "jsonrpc": "2.0",
            "id": 0,
            "method": "requestAccess",
            "params": {"clientId": client_id, "clientSecret": client_secret}
        }
        await websocket.send(json.dumps(request_access_request))
        request_access_response = await websocket.recv()
        print(f"Received request access response: {request_access_response}")

        
        # Authentication
        auth_request = {
            "jsonrpc": "2.0",
            "id": 1,
            "method": "authorize",
            "params": {
                "clientId": client_id,
                "clientSecret": client_secret,
                "license":"",
                "debit": 10
            }
        }

        await websocket.send(json.dumps(auth_request))
        auth_response = await websocket.recv()
        print(f"Received authentication response: {auth_response}")

        query_headset = {
            "jsonrpc":"2.0",
            "id":2,            
            "method":"queryHeadsets"
        }

        await websocket.send(json.dumps(query_headset))
        queryh = await websocket.recv()
        print(f"Headset Information:{queryh}")

asyncio.get_event_loop().run_until_complete(connect_and_communicate())

async def send_receive(websocket, message, description):
    await websocket.send(json.dumps(message))
    response = await websocket.recv()
    print(f"Received {description}: {response}")
    return json.loads(response)

async def create_session(websocket, cortex_token, headset_id):
    create_session_message = {
        "id": 3,
        "jsonrpc": "2.0",
        "method": "createSession",
        "params": {
            "cortexToken": cortex_token,
            "headset": headset_id,
            "status": "active"
        }
    }
    response = await send_receive(websocket, create_session_message, "Session Creation")
    print(f"Response from create_session: {response}")

    result_data = response.get('result', None)
    if result_data is not None:
        return result_data.get('id', None)
    else:
        print("Error: 'result' key not found in the response.")
        return None

async def subscribe_to_streams(websocket, cortex_token, session_id, streams):
    subscribe_message = {
        "id": 5,
        "jsonrpc": "2.0",
        "method": "subscribe",
        "params": {
            "cortexToken": cortex_token,
            "session": session_id,
            "streams": streams
        }
    }
    await send_receive(websocket, subscribe_message, "Subscription")

async def setup_profile(websocket, cortex_token, headset_id, profile_name):
    setup_profile_message = {
        "id": 6,
        "jsonrpc": "2.0",
        "method": "setupProfile",
        "params": {
            "cortexToken": cortex_token,
            "headset": headset_id,
            "profile": profile_name,
            "status": "load"
        }
    }
    response = await send_receive(websocket, setup_profile_message, "Profile Setup")
    print(f"Response from setup_profile: {response}")


cycle_letters = itertools.cycle(['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'])

# Add a cooldown duration for facial expressions
facial_expression_cooldown = 0.25  
last_facial_expression_time = time.time()

def handle_facial_expressions(data, label, entry):
    global current_letter, last_blink_time, word_buffer, entry_index, last_facial_expression_time
    double_blink_threshold = 0.5

    current_time = time.time()
    if current_time - last_facial_expression_time < facial_expression_cooldown:
        return  

    if 'fac' in data:
        fac_data = data['fac']
        print("Received facial expressions data:", fac_data)

        if fac_data and fac_data[0] == 'blink':
            if current_time - last_blink_time > double_blink_threshold:
                
                current_letter = next(cycle_letters)
                last_blink_time = current_time
                label.config(text=f"Blinked, Next Letter: {current_letter}")

        elif fac_data and fac_data[0] == 'winkL':
            
            entry_index += 1
            label.config(text=f"Wink Left, Next Letter: {current_letter}")
            entry.delete(entry_index, tk.END)
            entry.insert(tk.END, current_letter)
            entry_index = entry.index(tk.END)

        elif fac_data and fac_data[0] == 'winkR':
            
            label.config(text="Wink Right, Typed Space")
            entry.insert(tk.END, ' ')

        elif fac_data and fac_data[3] == 'smile':
            
            label.config(text="Smiled, Deleted Character")
            entry.delete(entry.index(tk.END) - 1, tk.END)

        elif fac_data and fac_data[3] == 'clench':
            text_to_read = entry.get()
            label.config(text="Clenched Teeth, Read Text")
            text_to_speech_engine.say(text_to_read)
            text_to_speech_engine.runAndWait()

        else:
            label.config(text="Neutral")

        last_facial_expression_time = current_time 

async def main():
    uri = "wss://localhost:6868"
    async with websockets.connect(uri) as websocket:
        print(f"Connected to {uri}")

        cortex_token = "CORTEX_TOKEN"  
        headset_id = "Headset_ID"
        profile_name = "Profile_Name"

        root = tk.Tk()
        root.title("Facial Expression Keyboard")

        entry = tk.Entry(root, width=30)
        entry.grid(row=0, column=0, columnspan=10, pady=10)

        label = tk.Label(root, text="", font=("Helvetica", 16))
        label.grid(row=1, column=0, columnspan=10, pady=20)

        keys = [
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
            'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P',
            'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L',
            'Z', 'X', 'C', 'V', 'B', 'N', 'M', 'Space', 'Backspace'
        ]

        row_val = 2
        col_val = 0

        for key in keys:
            if key == 'Space':
                button = tk.Button(root, text=key, width=5, height=2, command=lambda k=key: entry.insert(tk.END, ' '))
            elif key == 'Backspace':
                button = tk.Button(root, text=key, width=5, height=2, command=lambda k=key: entry.delete(entry.index(tk.END) - 1, tk.END))
            else:
                button = tk.Button(root, text=key, width=5, height=2, command=lambda k=key: entry.insert(tk.END, k))

            button.grid(row=row_val, column=col_val)
            col_val += 1
            if col_val > 9:
                col_val = 0
                row_val += 1

        await setup_profile(websocket, cortex_token, headset_id, profile_name)

        # CSV file for writing
        with open("facial_expression_data.csv", "w", newline="") as csvfile:
            csv_writer = csv.writer(csvfile)
            csv_writer.writerow(['Expression_Type', 'Engagement', 'Other_Columns'])

            session_id = await create_session(websocket, cortex_token, headset_id)

            await subscribe_to_streams(websocket, cortex_token, session_id, ["fac"])

            while True:
                response = await websocket.recv()
                data = json.loads(response)
                handle_facial_expressions(data, label, entry)
                root.update()

if __name__ == "__main__":
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main())
    loop.run_forever()
