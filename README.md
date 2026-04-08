
# Parody Studios Assignment

## Author

### **Sushant Jadhav** 
Game Developer | Unity Developer | Game Designer  
This repository contains my submission for the Parody Studios game assignment. The project is developed using **Unity** version 6.0.58 LTS and focuses on implementing modular, scalable gameplay systems with clean architecture and industry-standard coding practices.

---

## Overview

The project demonstrates the implementation of multiple core gameplay systems, designed with flexibility and maintainability in mind. Emphasis has been placed on:

* Decoupled system architecture
* Reusable components
* Clean code and documentation
* Design pattern usage

---

## Controls for playing the Game

- WASD / Arrow Keys → Movement  
- Mouse → Camera  
- Space → Jump
- E key to Enter Gravity Mode
- Arrow keys to Select Gravity Direction
- Enter Key to Confirm Gravity

--- 

## Project Structure

- Scripts/
    - Player/
        - Rigid Body Controller/
        - Camera Systems/
    - Gravity System/
    - Collectiible System/
- Collection System/ (Data Folder)

--- 

## Implemented Systems

### Player Controller

* Rigidbody-based movement system
* Custom **artificial gravity** handling
* Smooth and responsive player motion

### Camera Controller

* Handles camera rotation and directional control
* Designed for seamless integration with player movement
* Ensures consistent gameplay perspective

### Gravity Controller

* Central system responsible for:

  * Artificial gravity generation
  * Direction management
  * Hologram projection logic
* Built to be extensible for future gameplay mechanics

### Collectible System

* ScriptableObject-driven data architecture
* Runtime collectible handling
* Integrated UI management system

---

## Architecture & Design Patterns

The project is structured with scalability and maintainability in mind:

* **Observer Pattern**
  Enables loose coupling between systems and event-driven interactions

* **Singleton Pattern**
  Used where global access is required while maintaining control over instances

* **Modular Design**
  Systems are independent and reusable

* **Clean Code Practices**

  * Readable and maintainable code structure
  * Proper naming conventions
  * Inline documentation for clarity

---

## Builds

Pre-built versions of the project are available in the `Game Builds` folder.

### Windows

1. Unzip the provided folder
2. Run the `.exe` file

### macOS

Follow this guide to run the build: [https://youtu.be/2DTH7aYHQOs](https://youtu.be/2DTH7aYHQOs)

---

## Development Practices

* Well-documented scripts for clarity and maintainability
* Organized project structure
* Focus on performance-conscious implementation
* Scalable systems for future expansion

---

## Notes

* Detailed implementation logic can be found within the respective scripts
* The project is designed to showcase both **technical ability** and **system design thinking**

---



