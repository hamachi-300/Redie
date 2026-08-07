# 2D Modular Weapon Animation Guide

This guide explains how to create, record, and configure animations for different weapon types in **Re:DIE** using Unity's **Animator Override Controllers** and the **Visual Record Method**.

---

## The Workflow Overview
To avoid drawing separate pixel art animations for every weapon, we use **modular layering**:
1. The **Player's animations** control the body sprite and move the parent **`WeaponPivot`** (acting as the player's hand).
2. The **Weapon sprite** sits on a child placeholder GameObject (**`WeaponVisual`**) which inherits all movement from the hand pivot.
3. We use **Animator Override Controllers** to swap out the specific visual swing clips for each weapon dynamically.

---

## Step 1: Base Animator Controller Setup
Your main **`Player`** Animator Controller acts as the logical template:
1. In the **Parameters** tab, create these parameters:
   * **`Attack`** (Trigger)
   * **`IsHeavyAttack`** (Bool)
   * **`LightAttackSpeed`** (Float, Default = `1`)
   * **`HeavyAttackSpeed`** (Float, Default = `1`)
2. In the Animator grid, create two states:
   * **`LightAttack`** (Assign a default unarmed clip like `Player_Punch`).
     * Bind its **Speed** multiplier in the Inspector to the **`LightAttackSpeed`** parameter.
     * In the **Tag** field at the top of the Inspector, type: `Attack`.
   * **`HeavyAttack`** (Assign a default unarmed clip like `Player_HeavyPunch`).
     * Bind its **Speed** multiplier to the **`HeavyAttackSpeed`** parameter.
     * In the **Tag** field at the top, type: `Attack`.
3. Set transitions from `Idle/Walk` to these attack states using the `Attack` and `IsHeavyAttack` parameters. Ensure **`Has Exit Time`** is unchecked for snappy inputs.

---

## Step 2: Creating Override Clips (The Duplicate Trick)
*DO NOT create blank animation clips from scratch.* If you do, the player's body will not face the correct direction when editing.
1. Find your base player animation clip in the Project window (e.g. `Player_IdleNorth.anim` or `Player_LightAttack.anim`).
2. Select it and press **`Ctrl + D`** (or `Cmd + D` on Mac) to duplicate it.
3. Rename the duplicated file to match your new weapon (e.g. **`Club_LightAttack.anim`**).
4. Since this is a copy, it already contains the keyframes for the player's body sprites.

---

## Step 3: Recording Weapon Swings
1. In the Hierarchy, select **Player**.
2. Temporarily drag your new **`Club_Override`** controller asset into the Player's **`Animator`** component slot. (This lets you preview the club animations in the editor).
3. Open the **Animation** window (`Window > Animation > Animation`).
4. Select the clip you want to edit (e.g. `Club_LightAttack`).
5. Click the red **Record button (🔴)**.
6. Select the **`WeaponVisual`** child GameObject in the Hierarchy:
   * **Frame 0:** Position = `(0,0,0)`, Rotation = `0`.
   * **Wind-up Frame:** Move/rotate the weapon back.
   * **Strike Frame:** Rotate the weapon forward fast. Set **`Order in Layer`** (e.g., `-1` for North, `1` for South).
   * **Recovery Frame:** Return the weapon to the resting position.
7. Click the **Record button (🔴)** to stop.
8. **CRITICAL:** Go back to your Player's Animator component and put the original **`Player`** controller back in the slot.

---

## Step 4: Timing & Speed Multipliers
Timings are key to combat game feel. In Unity, the timeline runs at **60 frames per second (fps)**:

### Light Attack (Snappy) - ~10 Frames Total (0.16 seconds)
* **Frame 0:** Stance default.
* **Frame 3:** Wind-up (pull back to `-30` degrees).
* **Frame 6:** Strike (rotate forward to `90` degrees).
* **Frame 10:** Recovery (return to `0` degrees).

### Heavy Attack (Weighty) - ~30 Frames Total (0.50 seconds)
* **Frame 0:** Stance default.
* **Frame 15:** Slow wind-up (raise weapon overhead, e.g. `-100` degrees).
* **Frame 18:** Slash (slam flat to the ground in 3 frames, e.g. `120` degrees).
* **Frame 24:** Linger (freeze weapon flat on ground to show weight).
* **Frame 30:** Recovery (slowly lift weapon back to center).

---

## Step 5: Troubleshooting Checklist
If the weapon is not appearing or animating correctly at runtime:
* **Is the weapon invisible?** Check that your `WeaponData` ScriptableObject has the weapon sprite assigned, the player has the `WeaponVisual` assigned in the Inspector, and the `Local Scale` in the weapon data asset is not `(0,0,0)`.
* **Is the weapon not animating?** Make sure you dragged your `Weapon_Override` controller asset into the **`Animator Override`** slot of your `WeaponData` ScriptableObject.
* **Is the weapon stuck behind the player?** Remember that any property animated in *one* clip must be keyframed in *every* clip of the Blend Tree, or Unity will lock the value. Make sure `Order in Layer` is keyframed on Frame 0 of all clips.
