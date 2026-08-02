# Re:DIE - Agile Roadmap

## Milestone A: Core Gameplay & Mechanics MVP

- [x] **Sprint A1: Movement & Controls**
  - [x] WASD-based 2D movement (supporting both Rigidbody2D and Transform fallback).
  - [x] Unity 2021.3.28f1 LTS setup.
  - [x] Universal Render Pipeline (URP) configuration for 2D lighting.
  - [x] Editor utility for Auto-Save before entering Play Mode.

- [ ] **Sprint A2: Core Attacking & Combat Feedback**
  - [x] Basic attack inputs (light and heavy attacks).
  - [x] Stamina consumption for attacks
  - Trigger collider-based weapon hitboxes.
  - Combat game feel elements: screen shake, knockback, and impact freeze.

- [ ] **Sprint A3: Platforming, Jumping & Jump Attacks**
  - Physics-based jumping mechanics (handling variable jump height and gravity scale).
  - Jump attack implementation (combines jump state with core attack hitbox/damage logic).
  - Stamina consumption for jumping (optional Souls-like balance tuning).

- [ ] **Sprint A4: Dodge & Defense Mechanics**
  - Dodge roll / dash execution with temporary invincibility frames (i-frames).
  - Defense block input (e.g., Right Click or Shift) with damage mitigation.
  - Guard stamina consumption and shield active visual feedback.
  - Parry mechanic (negates damage and staggers attacker if timed perfectly).

---

## Milestone B: Souls-Like Systems & Content

- [ ] **Sprint B1: HP Potion & Inventory Items**
  - Quick-item inventory slot (UI display and selection).
  - HP Potion (Estus flask style) with limited charges that replenish on clearing a map section or starting a new run.
  - Potion drinking animation state (player is vulnerable while healing).

- [ ] **Sprint B2: Enemies & Advanced Movesets**
  - Base enemy AI (patrolling, line-of-sight tracking, alert states).
  - Enemy movesets (multi-hit combos, telegraphing indicators before attack, recovery windows).
  - Enemy health bars and impact visual feedback.

- [ ] **Sprint B3: Boss Encounter & Moveset**
  - Boss room entry triggers and dynamic boss health bar UI.
  - Boss moveset phase 1 (sweep attacks, overhead slams, ranged shockwaves).
  - Boss moveset phase 2 (enraged state, faster attacks, fire trails, or new patterns).

- [ ] **Sprint B4: Rogue-lite Progression & Real-time Auto-Save**
  - Real-time Auto-Save system (saves coins, level, and stats instantly upon acquisition/upgrade).
  - Coin/Currency API (system to collect coins from defeating enemies and environmental breakables).
  - Character Upgrades API (functions to level up max health, stamina, attack power, and speed using collected coins).

---

## Milestone C: Level Design, UI, & Game Loop

- [ ] **Sprint C1: Map Design, Textures & Level Traps**
  - Interconnected map section layouts (2D platformer rooms/arenas).
  - Map Design (Tilemap collisions, room transitions, and platform configurations).
  - Map Texture (Importing spritesheets, creating 2D tilesets, palette setup, and URP 2D lighting materials).
  - Environmental hazards/traps (spikes, falling boulders, temporary collapsing platforms).

- [ ] **Sprint C2: Menus & UI Panels**
  - **Main Menu Screen:** Start Game, Upgrade Character, and Exit options.
  - Character Upgrade Shop UI (spend coins collected in previous rounds to get stronger).
  - Pause Menu with resume, settings, and main menu fallback.
  - **Game Over & Respawn Loop:** "YOU DIED" screen that resets the player to the beginning point of the map with their persistently saved progress.
  - **End Credits Screen:** A scrolling credit screen triggered upon defeating the Boss, presenting victory stats and returning the player to the Main Menu.
  - Dynamic HUD displaying Health, Stamina, HP potion count, and current Coin balance.

- [ ] **Sprint C3: Audio & Sound Design**
  - **Audio Manager System:** Singleton/persistent manager to coordinate BGM transitions, volume controls, and SFX mixing.
  - **Action SFX:** Sound effects for player mechanics (attacks, jumps, land, dodge rolls, defense blocks, and potion drinking).
  - **Background Music (BGM):** Atmospheric, looping tracks for the main menu, explore map sections, and an intense orchestral track for the boss fight.
  - **Death SFX:** A heavy, echoey crash/doom sound effect to play immediately upon death during the "YOU DIED" screen transition.

