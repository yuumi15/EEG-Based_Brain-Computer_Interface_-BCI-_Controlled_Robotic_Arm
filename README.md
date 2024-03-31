# EEG-Based_Brain-Computer_Interface_-BCI-_Controlled_Robotic_Arm

### This project addresses the critical challenge of enhancing independence and communication for individuals with severe motor impairments, particularly those suffering from conditions like Amyotrophic Lateral Sclerosis (ALS) or spinal cord injury. It presents two parts: A novel Brain-Computer Interface (BCI)-controlled robotic arm system and a BCI-based virtual keyboard. (BSC. Graduation Project)

![Robotic Arm](https://i.imgur.com/a/6ZhX8R4)

## Abstract

### This project tackles the challenge of improving quality of life for people with severe motor impairments (e.g., ALS, spinal cord injury). We present a novel Brain-Computer Interface (BCI)-controlled robotic arm system for physical interaction.  BCI uses brain signals to control devices.  Our system uses non-invasive EEG signals and aims for affordability and accuracy.  We successfully integrated two headsets for separate functions: NeuroSky MindWave for robotic arm control and Emotiv EPOC for verbal communication via a BCI keyboard. The project demonstrates the potential of BCI technology to enhance accessibility and independence for individuals with severe motor limitations.

###   • Target users: Individuals with severe motor impairments
###   • Technology: BCI-controlled robotic arm system with separate headsets for physical and verbal communication
###   • Benefits: Improved quality of life, accessibility, and independence
###   • Outcomes: Successful integration and demonstration of potential

## Project Components:

### I. BCI-Controlled Robotic Arm:
### Utilizes the NeuroSky MindWave headset for non-invasive brain signal capture.
### Employs C# code (HelloEEG_NeuroskyBCI.cs) for communication, data processing, blink detection, and real-time visualization of blink strength.
### Integrates with an Arduino code (MotorInterface_ArmMovements.ino) that governs the robotic arm movements via servo motors based on received data.

### II. BCI-Based Virtual Keyboard:
### Leverages the Emotiv EPOC headset (14 channels) to capture EEG and facial expression data.
### Employs Python script (VirtualKeyboard_EmotivEPOC.py) to:
###   • Establish a real-time connection with the headset.
###   • Process brainwave patterns and facial gestures.
###   • Translate those signals into keyboard commands.
###   • Utilize Tkinter library for a user-friendly GUI displaying a virtual keyboard.
###   • Integrate Pyttsx3 library for text-to-speech functionality, vocalizing user input.


