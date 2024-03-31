#include <Servo.h>
// Define the servos for each finger and wrist
Servo thumb;
Servo index;
Servo middle;
Servo ring;
Servo pinky;
Servo wrist;
int lastButtonState= HIGH; ;   
int currentButtonState; 
const int BUTTON_PIN = 12;
int motionState = 0;  
void setup() {
  Serial.begin(115200);
  // Attach servos to pins
  thumb.attach(2);
  index.attach(3);
  middle.attach(4);
  ring.attach(5);
  pinky.attach(6);
  wrist.attach(8);
  restPosition();  // Set initial position
  // Set the wrist servo to an initial position
  wrist.write(90);
  pinMode(BUTTON_PIN, INPUT_PULLUP); // set arduino pin to input pull-up mode
  currentButtonState = digitalRead(BUTTON_PIN);
}
void restPosition() {
  thumb.detach();
  index.detach();
  middle.detach();
  ring.detach();
  pinky.detach();
  wrist.write(90);
}
void openFist() {
  thumb.attach(2);
  index.attach(3);
  middle.attach(4);
  ring.attach(5);
  pinky.attach(6);
  thumb.write(0);
  index.write(0);
  middle.write(0);
  ring.write(0);
  pinky.write(0);
  wrist.write(0);
}
void closedFist() {
  thumb.attach(2);
  index.attach(3);
  middle.attach(4);
  ring.attach(5);
  pinky.attach(6);
  thumb.write(180);
  index.write(180);
  middle.write(180);
  ring.write(180);
  pinky.write(180);
  wrist.write(90);
}
void thumbsUp() {
  thumb.attach(2);
  index.attach(3);
  middle.attach(4);
  ring.attach(5);
  pinky.attach(6);
  thumb.write(0);
  index.write(180);
  middle.write(180);
  ring.write(180);
  pinky.write(180);
  wrist.write(0);
}
void victory() {
  thumb.attach(2);
  index.attach(3);
  middle.attach(4);
  ring.attach(5);
  pinky.attach(6);
  thumb.write(180);
  index.write(0);
  middle.write(0);
  ring.write(180);
  pinky.write(180);
  wrist.write(180);
}
void loop() {
   currentButtonState = digitalRead(BUTTON_PIN); 
  if( currentButtonState == LOW) {
    Serial.println("ButtonPressed;");    
  }
  // Check if data is available to read from the serial port
  if (Serial.available() > 0) {
    // Read the command sent from C#
    String command = Serial.readStringUntil(';');
    // Convert the command to an integer
    int value = command.toInt();
    // Update motion state based on the received command
    if (command.startsWith("1")) {
      motionState = 1;  // Victory
    } else if (command.startsWith("2")) {
      motionState = 2;  // Closed Fist
    } else if (command.startsWith("3")) {
      motionState = 3;  // Open Fist
    } else if (command.startsWith("4")) {
      motionState = 4;  // Thumbs Up
    }
  }
  // Execute servo movements based on the current motion state
  switch (motionState) {
    case 1:
      victory();
      delay(2000);
      restPosition();
     motionState = 0;  // Reset motion state
      break;
    case 2:
      closedFist();
      delay(2000);
      restPosition();
      motionState = 0;  // Reset motion state
      break;
    case 3:
      openFist();
      delay(2000);
      restPosition();
      motionState = 0;  // Reset motion state
      break;
    case 4:
      thumbsUp();
      delay(2000);
      restPosition();
      motionState = 0;  // Reset motion state
      break;
    default:
      // No motion, do nothing
      break;
  }
}
