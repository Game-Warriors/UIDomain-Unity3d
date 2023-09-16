  version : 0.7.1
  fix closing screen event trigger place. udpate readme

  version : 0.7.0
  refactoring UI system structure and add more single responsible abstraction, decuple UI system from UI screen items class.

  version : 0.6.9
  add private properties inject on UI System when screen is instantiated

  version : 0.6.8
  polish code structure, improve editor UI and fix clear screen not call OnClose bug

  version : 0.6.7
  Fix compatibility to old unity dotnet

  version : 0.6.6
  add search feature into ui system editor, add changeable variable for black panel fade in/out time

  version : 0.6.5
  fix ignoring show open animation after close has black screen screens

  version : 0.6.4
  add passing onClose deceleration argument event in show method

  version : 0.6.3
  add option in OnRequestCloseScreen method for showing open animation when close screen using back button

  version : 0.6.2
  Fix show close animation bug
  
  version : 0.6.1
  Fix editor UI elements adjustment bug
  
  version : 0.6.0
  Added capability to UI system which it could set the main and screen canvas into UI main config and instantiate canvases if there wasn't exist in runtime.

  version : 0.5.0
  Added new method to IScreen interface to change canvas camera and then rise camera change event by passing new camera in IUIEventHandler.