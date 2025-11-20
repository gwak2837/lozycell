# Lozycell Setup Instructions

## 1. Scene Setup

Create two scenes in `Assets/Scenes/`:

1.  **BaseScene** (This will be the starting scene)
2.  **ArcadeScene**

Add both scenes to **File > Build Settings**.

## 2. Tags

Go to **Edit > Project Settings > Tags and Layers** and add a new Tag: `AminoAcid`.

## 3. BaseScene Setup

1.  **GameManager**:
    - Create an Empty GameObject named `GameManager`.
    - Attach the `GameManager` script.
2.  **UI**:
    - Create a Canvas.
    - Add Text (TextMeshPro) for: Amino Count, Mitochondria Level, Attack Power.
    - Add Buttons for: "Upgrade Mitochondria", "Go to Arcade".
3.  **BaseManager**:
    - Create an Empty GameObject named `BaseManager`.
    - Attach the `BaseManager` script.
    - Drag and drop the UI elements (Texts and Buttons) into the script slots in the Inspector.

## 4. ArcadeScene Setup

1.  **Player**:
    - Create a 2D Sprite (e.g., a Circle).
    - Add `Rigidbody2D` (set Gravity Scale to 0).
    - Add `BoxCollider2D`.
    - Attach `PlayerController` script.
2.  **Amino Acid Prefab**:
    - Create a 2D Sprite (e.g., a small Hexagon).
    - Add `CircleCollider2D` (Check **Is Trigger**).
    - Set Tag to `AminoAcid`.
    - Attach `AminoAcid` script.
    - Drag into Project window to make a Prefab.
3.  **ArcadeManager**:
    - Create an Empty GameObject named `ArcadeManager`.
    - Attach `ArcadeManager` script.
    - Assign the **Amino Acid Prefab** to the script.
    - Assign the **Progress Text** (create one in Canvas).
    - Create an Empty GameObject "SpawnCenter" and assign it to `Spawn Area`.

## 5. Testing

1.  Start in **BaseScene**.
2.  Click "Go to Arcade".
3.  Collect 100 Amino Acids.
4.  Observe auto-return to Base.
5.  Check if Amino Acids increased.
6.  Click "Upgrade" and verify Level/Attack stats increase.
