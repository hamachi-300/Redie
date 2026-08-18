# Re:DIE

A challenging 2D single-player Souls-like action roguelike developed in Unity.

## 🎮 Game Description

**Re:DIE** challenges players to explore dangerous, trap-ridden map sections, master high-stakes combat, and survive encounters with standard enemies and imposing bosses.

### Core Game Pillars & Features:
* **Souls-Like Combat:** 
  * Tactical combat utilizing attacking, jumping, dodge rolling (with i-frames), and active defense/blocking.
  * Jumping attacks for dealing damage from mid-air.
  * Stamina management governing your movement, defensive options, and attacks.
* **Complex Enemy & Boss Movesets:**
  * standard enemies and bosses feature advanced AI with custom movesets (combos, telegraphed special attacks, recovery windows, and multi-phase boss transitions).
* **Survival, Items & Persistent Progression:**
  * Quick-item slot system and replenishable HP Potions (Estus-style) to heal damage.
  * Persistent Coin API: Collect coins from enemies and breakable objects.
  * Real-time Auto-Save: Player progress (coins, levels, stats) is saved instantly, ensuring zero progress loss.
  * Character Level-Up Shop: Spend collected coins to upgrade maximum Health, Stamina, Attack Power, and Speed.
* **Level & Map Design:**
  * Interconnected map section layouts with precise platform configurations and tilemap collisions.
  * Stylized map textures configured via 2D spritesheets, tilesets, and URP 2D lighting materials.
  * Environmental level hazards and traps (spikes, falling boulders, and collapsing structures).
* **Polished Game Flow & Respawn Loop:**
  * Complete Main Menu system with character upgrade shop integration.
  * "GAME OVER" screen resetting the player back to the starting point of the map on death, while retaining all acquired coins and stats for their next round.
  * Victory End Credits scroll triggered upon defeating the final Boss.
* **Audio & Sound Design:**
  * Action sound effects (SFX) for player movement, swings, rolls, blocks, and potion usage.
  * Loopable background music (BGM) for explore stages and aggressive orchestration for boss encounters.
  * Dramatic, heavy death SFX synchronized with the "YOU DIED" screen.
* **Atmospheric 2D Visuals:** Atmospheric graphics utilizing Unity's Universal Render Pipeline (URP) with custom 2D lighting.

---

## ⚙️ How to Clone & Run the Project

Since this project manages Unity assets, follow these setup steps:

### 1. Install Git LFS (Required)
This project uses **Git Large File Storage (LFS)** to manage large scene files and spritesheets. 

Before cloning, you **must** install Git LFS on your system, or your scenes and sprites will be downloaded as broken 1 KB pointer files:
1. Download and run the installer from **[git-lfs.github.com](https://git-lfs.github.com)**.
2. Open your terminal (PowerShell, Command Prompt, or Git Bash) and run this command once:
   ```bash
   git lfs install
   ```

### 2. Clone the Repository
Clone the repository using HTTPS or SSH:
```bash
git clone https://github.com/hamachi-300/Redie.git
```
*(Git LFS will run in the background and automatically download all heavy textures, audio, and scene files during the cloning process).*

### 3. Open in Unity
1. Open **Unity Hub**.
2. Click **Add project from disk**.
3. Select the cloned `Redie` directory.
4. Launch the project. *(Note: First load will take a few minutes as Unity generates the local cache/library folders).*

#### Unity 2021.3 LTS (2021.3.28f1)

