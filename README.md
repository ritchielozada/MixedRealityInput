## Mixed Reality Input

InteractionManager Example for Microsoft Mixed Reality Immersive Headsets with Motion Controllers, Xbox One Gamepad and Chatpad

### Developer Tools Version

- Unity 2017.2.0f3
- Windows SDK 10.0.16299.0
- Windows 10 Version 1709 OS Build 16299.15

### Hardware

- Mixed Reality Immersive Headset (Tested on Acer and Samsung Odyssey)
- Mixed Reality Motion Controllers
- Xbox One Controller Gamepad connected via USB cable
- Xbox One Controller Chatpad connected via USB cable

### Warning / Notice

- The sample supports user controlled movement and view change in the virtual space supplementing head tracking on the HMD (head mounted display) with the thumbsticks instead of the typical teleporting with fade-out or pre-set movement points.
- Some users may experience nausea, dizziness, or similar symptoms with the user controlled movement.

### Notes

1. Creates a "Shield" object when Left Motion Controller is detected.
2. Creates a "Sword" object when Right Motion Controller is detected
3. Left Thumbstick (Motion Controller and Xbox Gamepad) moves the user forward, back, left, right.
4. Right Thumbstick (Motion Controller and Xbox Gamepad) turns view up, down (pitch), left, right (yaw)
5. Movement is based on the virtual user body instead of the current user viewpoint (like being on top of a tank and turret)


### Code Notes

1. The Motion Controller's center/released values may vary across devices; this is referred to as the joystick deadzone -- the threshold before input is considered valid.
2. A Turn threshold is also setup to minimize unnecessary camera rotation updates.
3. The Xbox One Controller Chatpad is recognized as keyboard input when connected via USB cable.  The keystroke behavior difference to standard keyboard is the key down and key up events; a keystroke on the Chatpad will trigger the key-down and key-up events immediately and holding the key down will create repeating key-down and key-up events -- in contrast to a standard keyboard keystroke that has a key-down, key-pressed and key-up events matching the user action.
