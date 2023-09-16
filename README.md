# UIDomain-Unity3d
![GitHub](https://img.shields.io/github/license/svermeulen/Extenject)

# Table Of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<summory>

  - [Introduction](#introduction)
  - [Features](#features)
  - [Installation](#installation)
  - [Startup](#startup)
  - [How To Use](#how-to-use)
</summory>

# Introduction
This library provides the UI page stacking feature inside Unity3d engine and based on UI canvas. also provide the black background, the screen locking, create toast and managing page screen prefabs. it’s implemented fully by C# language. The library has simple editor to setup screen prefabs and other features.

![Figure 1](../media/Images/Figure1.jpg?raw=true)

Support platforms: 
* PC/Mac/Linux
* iOS
* Android
* WebGL
* UWP App


* This library is design to be dependecy injection friendly, the recommended DI library is the [Dependency Injection](https://github.com/Game-Warriors/DependencyInjection-Unity3d) to be used.

```
```
This library used in the following games and apps:
</br>
[Street Cafe](https://play.google.com/store/apps/details?id=com.aredstudio.streetcafe.food.cooking.tycoon.restaurant.idle.game.simulation)
</br>
[Idle Family Adventure](https://play.google.com/store/apps/details?id=com.aredstudio.idle.merge.farm.game.idlefarmadventure)
</br>
[CLC BA](https://play.google.com/store/apps/details?id=com.palsmobile.clc)

# Features
* MVVM architecture
* Screens stack management
* Back button and escape operation support in stack pipeline
* Screen property injection
* Screens instance and life management
* Toast rising and instance management
* Multiply canvas supporting

# Installation
This library can be added by unity package manager form git repository or could be downloaded.
for more information about how to install a package by unity package manager, please read the manual in this link:
[Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# Startup
After adding package to using the library features, following objects have to Initialize.

* UISystem: This class provide all system feature corresponding to IUIAspectRatio, IToast, IScreenStack , IUIOperation abstractions. this class also detect and call UI event on IUIEventHandler abstraction.

    * IUIAspectRatio: The base abstraction which presents some useful values for fitting and adjusting UI items

    * IToast: The base abstraction which presents screen showing toast features which is specify by visibility time and screen point.

    * IScreenStack:  The base abstraction which presents screen stack features like, show, close, working with black screen and control screen remove by system back.

* DefaultResourseLoader: This class provide resource loading for UI system.

* DefaultDependencyInjector: This class provide property injection for screen items, by using service provider.

* UIManager: This class setup and start application UI pipeline, Handle UI events if any actions need to be triggered, show and handle UI basic toasts, manage special UI event like closing UI bars.

```csharp
public class UIManager : MonoBehaviour, IUIEventHandler
{
      private IUIOperation _uiOperation;

      public void Initialization(IScreenStack screen, IToast toast, IUIOperation uiOperation)
      {
          UIScreenItem.Initialization(screen, ServiceProvider, toast);
      }

      void IUIEventHandler.OnToastRises(float showTimeLength, IToastItem toast)
      {
      }

      void IUIEventHandler.OnCloseLastScreen()
      {
      }

      void IUIEventHandler.OnShowScreen(IScreenStack screen)
      {
      }

      void IUIEventHandler.OnCloseScreen(IScreenStack screen)
      {
      }

      void IUIEventHandler.OnOpenScreen(IScreenStack screen)
      {
      }

      void IUIEventHandler.OnHideScreen(IScreenStack screen)
      {
      }

      void IUIEventHandler.OnScreenForceClose(IScreenStack screen)
      {
      }
      
      void IUIEventHandler.OnCanvasCameraChange(Camera newCamera)
      {
      }
      
      private void Update()
      {
          _uiOperation.SystemUpdate();
      }
}
```
In order to initialize UI system, the “UISystem” should constructed, and other required instances should construct and passed to “UISystem” instance. after creating storage system, the WaitForLoading method should called. in the WaitForLoading all loading resource will do and because this operation is blocking process, its return task object. like following example.

```csharp
private void Awake()
{
    UIManager uiManager = new UIManager();
    IServiceProvider serviceProvider = new ServiceProvider();
    IUIResources uiResources = new DefaultResourseLoader();
    IDependencyInjector dependencyInjector = new DefaultDependencyInjector();
    UISystem uiSystem = new UISystem(uiManager, uiResources, dependencyInjector, serviceProvider);
    await uiSystem.WaitForLoading();
}
```
If the dependency injection library is used, the initialization process could be like following sample.
```csharp
private void Awake()
{
    serviceCollection = new ServiceCollection(INIT_METHOD_NAME);
    serviceCollection.AddSingleton<IScreenStack, UISystem>(input => input.WaitForLoading());
    serviceCollection.AddSingleton<IToast, UISystem>();
    serviceCollection.AddSingleton<IAspectRatio, UISystem>();
    serviceCollection.AddSingleton<IUIOperation, UISystem>();
}
```

# How To Use
In order to start using features is setup using its editor window. The steps are three items.
1.  __Create and configure canvases__
The library work base on unity UI and it’s canvas component. it’s use two canvas type:

    * <b>Main Canvas:</b> This is base and main canvas can parent objects which render as back or middle place layout. also, this canvas could be used for less change UI object to decrease UI rect recalculation.
    * <b>Screen Canvas:</b> The canvas could config and override layout which render over main canvas object. also, this canvas could be used for frequently change UI objects.

    The canvas prefabs have to created and assign into editor selection place (figure 3-1), or in another way add to main scene and tag as MainCanvas and ScreenCanvas (figure 3-2). to open the UI editor in Unity3D top navigation bar select “Tools/UI Main Config”.

![Figure 2](../media/Images/Figure2.jpg?raw=true)

![Figure 3](../media/Images/Figure3.jpg?raw=true)

2.  __Create and configure panels__
There are two type panels that is useful for user interface operation.

    * <b>Black Panel:</b> This panel use for dark background layout popup like screen which will in create and manager screen section.

    * <b>Lock Panel:</b> The lock panel using for the, situation where need all interaction block or consume by one layout. this panel by default is without graphics, visual or presentation.



3.  __Configure Toasts__
Toast is small notification message which shows somewhere middle of screen vertical line and disappear after time interval. it’s may have the fade or move animation during the interval. the toast feature could be access by IToast interface.

    * <b>Toast Prefab:</b> the game object which has attached a script component (MonoBehaviour) that has to implement the IToastItem Abstraction. final the created prefab from this game object could add to UI editor configuration to use.

    * <b>Toast Pool Count:</b> The Toast has visible time. this property defines how many toasts can be shown in same time.


## Creating and Setup Screens

Generally, there are two types of UI layout base on its own presentation, first is pages which is full rect or full screen layout and second is popups which is half-rect or layout just fill some part of screen. both will call the screen.

1.  __Create Prefab:__ create the empty game object and attached it as child to one of canvases and set the meanable name for game object, for example “MainMenuScreen”. select the game object, in Rect transform section change the alignment mode to stretch and fit the object bound to canvas width and height, create the C# script in specific folder for UI screen classes. The created class has to derived from abstract class “UIScreenItem”. add the created class to screen game object. finally drag the game object from scene to specific folder in project view and then delete the game object from scene.

2.  __Setup Prefab:__ open UI editor by “Tools/UI Main Config” path in navigation bar. find the “Add Screen Item” button and click, the one panel item has been created top of the button. the panel has input field one for screen name to call for screen in runtime and second is the screen prefab reference. after inputting the name and assign prefab, it should click the “Save Configuration Asset“(figure 4-2) button to save the changes.

![Figure 4](../media/Images/Figure4.jpg?raw=true)

3.  __Prepare Class Code:__  The system can work with Monobehavior class where drive from “IScreenItem” interface, there are base and ready to use abstract class which could be use as parent class instead of Monobehavior class. the “UIScreenItem” Abstract class has some implemented screen item feature plus useful virtual Propeties and methods which could be used or override.

    * OnInitialized: the method triggers in the screen first time call and after instantiation of screen prefab object.

    * OnShow: the method triggers each time screen enabled and shows in UI canvas otherwise presents top of all screens in as last screen.

    * OnHide: the method triggers when the new screen shows up and the screen goes behind the new screen. otherwise, the screen will queue in the screens stack to shows up again.

    * OnClose: the method triggers when screen close and remove from screen stack.

    * OnRequestCloseScreen: the method triggers when the screen request for closing by device back button. the screen could be close by condition if not be the last screen. 

## Using Screens
* Showing Screen:
    In order to show a screen object, the IScreenStack abstraction must be used. The “IScreenStack”  there are “ShowScreen” method inside this abstraction to call for showing. the method has four arguments to use:

    * ScreenName: the screen name which is booked in UI editor tools for each screen prefab.
    * CanvasType: the target canvas which desire the screen prefab attached to. it can be Main or Screen canvas.
    * PreviosScreenAct: indicate what happen to previous screen after showing desire screen.
    * OnClose: assigning the action method call to triggers after screen closed.

    ```csharp
    private void RunApp(IScreenStack ScreenStack)
    {
      MainMenuScreen mainMenuScreen 
          = ScreenStack.ShowScreen<MainMenuScreen>(MainMenuScreen.SCREEN_NAME, ECanvasType.MainCanvas, EPreviosScreenAct.ForceClose);
    }
    ```
* Closing Screen: 
The screens can be close by three way.

    *a.* closing screen by calling “CloseScreen” method in IScreenStack abstraction.

    *b.* closing screen by showing new screen and set the “EPreviousScreenAct“ property “Close or ForceClose”. The screen will not close when its be a last screen, to force the system to close last screen, the “EPreviousScreenAct“ should pass “ForceClose“

    *c.* closing screen by system back button. the system automaticly check the system back button click evet and close the current screen its be a last screen.