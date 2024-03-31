## EEG-Based Brain-Computer Interface (BCI) Controlled Robotic Arm

#### This project addresses the critical challenge of enhancing independence and communication for individuals with severe motor impairments, particularly those suffering from conditions like Amyotrophic Lateral Sclerosis (ALS) or spinal cord injury. It presents two main parts: A novel Brain-Computer Interface (BCI)-controlled robotic arm system and a BCI-based virtual keyboard. (BSC. Graduation Project)

##### Our Robotic Arm Showing Different Motion Execution
![Robotic Arm Executed Motions](https://imgur.com/enYOTtZ.png)
## Abstract

This project tackles the challenge of improving the quality of life for people with severe motor impairments (e.g., ALS, spinal cord injury). We present a novel Brain-Computer Interface (BCI)-controlled robotic arm system for physical interaction.  BCI uses brain signals to control devices.  Our system uses non-invasive EEG signals and aims for affordability and accuracy.  We successfully integrated two headsets for separate functions: NeuroSky MindWave for robotic arm control and Emotiv EPOC for verbal communication via a BCI keyboard. The project demonstrates the potential of BCI technology to enhance accessibility and independence for individuals with severe motor limitations.

• **Target users:** Individuals with severe motor impairments  
• **Technology:** BCI-controlled robotic arm system with separate headsets for physical and verbal communication   
• **Benefits:** Improved quality of life, accessibility, and independence       
• **Outcomes:** Successful integration and demonstration of potential

## Project Components:

#### I. BCI-Controlled Robotic Arm:
   •  Utilizes the NeuroSky MindWave headset for non-invasive brain signal capture.  
   •  Employs C# code *(HelloEEG_NeuroskyBCI.cs)* for communication, data processing, blink detection, and real-time visualization of blink strength.  
   •  Integrates with an Arduino code *(MotorInterface_ArmMovements.ino)* that governs the robotic arm movements via servo motors based on received data.  

##### EEG headset NeuroSky's Mindwave + electrode position
![NeuroSky MindWave](https://i.imgur.com/bLIdDUI.png)

#### II. BCI-Based Virtual Keyboard:
   Leverages the Emotiv EPOC headset (14 channels) to capture EEG and facial expression data.   
   Employs Python script *(VirtualKeyboard_EmotivEPOC.py)* to:   
   • Establish a real-time connection with the headset.   
   • Process brainwave patterns and facial gestures.   
   • Translate those signals into keyboard commands.   
   • Utilize Tkinter library for a user-friendly GUI displaying a virtual keyboard.   
   • Integrate Pyttsx3 library for text-to-speech functionality, vocalizing user input.  

##### Emotiv EEG headset with electrode positions in 10-20 system
![Emotiv Epoc](https://i.imgur.com/b7svuzI.png)


## Code Files:

• ***HelloEEG_NeuroskyBCI.cs (C#):*** Handles communication and data processing from the NeuroSky MindWave headset.   
• ***MotorInterface_ArmMovements.ino (Arduino):*** Controls the robotic arm movements through servo motors.   
• ***VirtualKeyboard_EmotivEPOC.py (Python):*** Implements the BCI-based keyboard using the Emotiv EPOC headset.    

