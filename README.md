<p align="center">
  <img src="https://github.com/Chandan-Baskey/RoyalRun-3dGame/blob/70612249410b81798817484b3e2f88db15be3e4d/Royal%20run.png?raw=true" alt="Escape The Cave banner" width="100%">
</p>

# 🏃‍♂️ Royal Runner

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-Gameplay%20Scripting-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Mobile-blue)
![Genre](https://img.shields.io/badge/Genre-3D%20Endless%20Runner-orange)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow)
![License](https://img.shields.io/badge/License-MIT-green)

A 3D lane-based endless runner built in Unity. Chunk-based procedural level generation moves the world toward the player, who dodges fences, avoids obstacles, and collects coins to rack up a distance-based score before losing all three lives.

---

## 📸 Screenshots
<p align="center">
  <img src="https://github.com/Chandan-Baskey/RoyalRun-3dGame/blob/7f435a76ac801ad85d3546c80cc743f10782f130/Start.jpg?raw=true?raw=true" width="600">
  <br><em>Start View </em>
</p>

| Gameplay | Game Over |
|---|---|
| ![Gameplay View](https://github.com/Chandan-Baskey/RoyalRun-3dGame/blob/7f435a76ac801ad85d3546c80cc743f10782f130/Runner%20Game%20View.jpg?raw=true) | ![Game Over View](https://github.com/Chandan-Baskey/RoyalRun-3dGame/blob/7f435a76ac801ad85d3546c80cc743f10782f130/Over%20View.jpg?raw=true) |

---

## ✨ Features

- **Procedural chunk-based track** — the level is built and recycled from repeating chunk prefabs rather than one long static path
- **Lane-based obstacle & fence placement** — each chunk randomly selects 0–2 of 3 lanes for fences, always leaving a coin-collectible lane open
- **Continuous obstacle spawner** — a second obstacle stream spawns independently within a configurable width, with random rotation
- **Distance/time-based scoring** — score climbs automatically the longer the run survives
- **Coin collection sub-score** — coins tracked and displayed separately from the main score
- **3-life health system** — colliding with an obstacle costs a life; hitting the game over threshold stops the run
- **Animator-driven hit reactions** — collisions trigger a `hit` animation state on the player
- **Game Over / Retry / Main Menu flow** — UI panels swap on death, track movement halts, and the player can restart or return to the main menu
- **New Input System movement** — player movement is driven by `InputAction.CallbackContext` rather than legacy `Input.GetAxis`

---

## 🧩 Architecture Overview

```
┌─────────────────────┐        ┌──────────────────────┐
│     MainUI.cs        │──────▶│  SceneManager.Load(1) │──▶ Gameplay Scene
│  (Main Menu buttons)│        └──────────────────────┘
└─────────────────────┘

                         GAMEPLAY SCENE
┌───────────────────────────────────────────────────────────────────┐
│                                                                     │
│   ┌────────────────┐        spawns chunks ahead        ┌────────┐ │
│   │ LevelGenerator  │──────────────────────────────────▶│ Chunks │ │
│   │  .cs            │        moves + recycles chunks     │  .cs   │ │
│   └────────────────┘◀────────────────────────────────── └────────┘ │
│                                                    spawns Fence/Coin │
│   ┌────────────────┐                                                │
│   │  Swpawner.cs    │────── spawns random Obstacles into world ────▶│
│   └────────────────┘                                                │
│                                                                       │
│   ┌────────────────┐   Move()   ┌────────────────┐                  │
│   │  Input System   │───────────▶│  Movement.cs    │  clamps + moves │
│   └────────────────┘            │  (Rigidbody)     │  the player      │
│                                  └────────────────┘                  │
│                                          │ collides with              │
│                                          ▼                            │
│                                 ┌──────────────────┐                 │
│                                 │ PlayerCollision   │                 │
│                                 │  .cs              │                 │
│                                 └──────────────────┘                 │
│                        Fence/Obstacle │      │ Coin (trigger)         │
│                                       ▼      ▼                         │
│                              animator.SetTrigger("hit")               │
│                              gm.ChangeLife(-1)     gm.ChangeCoin(10)  │
│                                                                       │
│   ┌────────────────┐        life <= 0                                │
│   │ GameManagement  │───────────────▶ GameOver() → GameOverUI shown   │
│   │  .cs            │        Restart() / MainMenu() → LoadScene      │
│   └────────────────┘                                                 │
│                                                                       │
│   ┌────────────────┐                                                 │
│   │ ObstaclesDestroy│── destroys anything tagged "Obstacle" that      │
│   │  .cs            │   enters its trigger (off-screen cleanup zone)  │
│   └────────────────┘                                                 │
└───────────────────────────────────────────────────────────────────┘
```

---

## 📜 Script Breakdown

### `MainUI.cs`
Handles the two buttons on the main menu.

| Method | Purpose |
|---|---|
| `StartGame()` | Loads the gameplay scene via `SceneManager.LoadScene(1)` |
| `QuitGame()` | Calls `Application.Quit()` |

```csharp
public void StartGame()
{
    SceneManager.LoadScene(1);
}
```

---

### `LevelGenerator.cs`
Drives the endless-runner illusion: spawns an initial run of chunks, then continuously moves and recycles them each frame.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `chunkPrefab` | `GameObject` | The chunk segment to spawn repeatedly |
| `chunkParent` | `GameObject` | Parent transform chunks are instantiated under |
| `initialChunksAmount` | `int` | How many chunks are pre-spawned at start (default `10`) |
| `chunkLength` | `int` | World-space length of one chunk, used for spacing and recycling (default `10`) |
| `moveSpeed` | `float` | Speed at which chunks move toward the camera (default `10`) |
| `isMoving` | `bool` (public) | Master switch for whether the track is scrolling |

**Flow**
1. `Start()` spawns `initialChunksAmount` chunks end-to-end along Z, then spawns one extra via `SpawnNewChunk()`.
2. `Update()` calls `MoveChunk()` every frame while `isMoving` is `true`.
3. `MoveChunk()` translates every chunk backward by `moveSpeed * Time.deltaTime`. Once a chunk passes behind `Camera.main.transform.position.z - chunkLength`, it's destroyed and a replacement is spawned at the front — creating the infinite-track effect.
4. `StopMoving()` is a public hook (called on death) that halts scrolling by setting `isMoving = false`.

```csharp
private void MoveChunk()
{
    for (int i = 0; i < chunks.Count; i++)
    {
        GameObject chunk = chunks[i];
        chunk.transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
        if (chunk.transform.position.z < Camera.main.transform.position.z - chunkLength)
        {
            chunks.Remove(chunk);
            Destroy(chunk);
            SpawnNewChunk();
        }
    }
}
```

---

### `Chunks.cs`
Lives on each chunk prefab. Randomly places fences across up to 3 lanes and always drops one coin in a lane that wasn't used for a fence.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `lanes` | `float[]` | Fixed lane X-positions: `{-2.8, 0, 2.8}` |
| `fance` | `GameObject` | Fence obstacle prefab |
| `coin` | `GameObject` | Coin pickup prefab |

**Flow**
1. `SpawnFance()` picks a random count between 0 and `lanes.Length - 1` (i.e., 0–2), then removes that many lanes from `availableLance` and instantiates a fence in each.
2. `SpawnCoin()` always spawns a coin in `availableLance[0]` — the first lane left untouched by fences — offset `+0.8f` on Y.

```csharp
int fencesToSpawn = Random.Range(0, lanes.Length);
for (int i = 0; i < fencesToSpawn; i++)
{
    if (availableLance.Count == 0) break;
    int randomIndex = Random.Range(0, availableLance.Count);
    int selectedLane = availableLance[randomIndex];
    availableLance.RemoveAt(randomIndex);
    Vector3 spawnPosition = new Vector3(lanes[selectedLane], transform.position.y, transform.position.z);
    Instantiate(fance, spawnPosition, Quaternion.identity, transform);
}
```

---

### `Swpawner.cs`
A second, independent obstacle stream — spawns obstacles on a coroutine loop at the spawner's own position.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `obstacles` | `GameObject[]` | Pool of obstacle prefabs to choose from randomly |
| `numToSpawn` | `int` | Intended spawn cap (default `5`) — see Known Issues |
| `waitPerSpawn` | `int` | Seconds between spawns (default `1`) |
| `spawnWidth` | `float` | Intended random X range for spawn position (default `3`) — see Known Issues |
| `obstacleParent` | `GameObject` | Parent transform for spawned obstacles |

```csharp
IEnumerator SpawmObstacles()
{
    while (true)
    {
        yield return new WaitForSeconds(waitPerSpawn);
        GameObject obstacle = obstacles[Random.Range(0, obstacles.Length)];
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnWidth, spawnWidth), transform.position.y, transform.position.z);
        Instantiate(obstacle, transform.position, Random.rotation, obstacleParent.transform);
        numToSpawn--;
    }
}
```

---

### `Movement.cs`
Player locomotion using the new Input System and `Rigidbody.MovePosition`, clamped to a playable rectangle.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `moveSpeed` | `float` | Movement speed (default `15`) |
| `clamp` | `Vector2` | X/Z bounds the player is clamped within |

```csharp
public void HandleMovement()
{
    Vector3 currentPosition = rb.position;
    Vector3 moveDirection = new Vector3(move.x, 0, move.y);
    Vector3 newPostion = currentPosition + moveDirection * (moveSpeed * Time.fixedDeltaTime);

    newPostion.x = Mathf.Clamp(newPostion.x, -clamp.x, clamp.x);
    newPostion.z = Mathf.Clamp(newPostion.z, -clamp.y, clamp.y);
    rb.MovePosition(newPostion);
}
```
`Move(InputAction.CallbackContext)` is wired to a `PlayerInput` component's move action and reads a `Vector2` each callback.

---

### `PlayerCollision.cs`
Central collision handler tying the player into both the animator and `GameManagement`.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `animator` | `Animator` | Player's animator, triggers `"hit"` on collision |
| `gm` | `GameManagement` | Must be assigned manually in the Inspector — see Known Issues |

| Event | Tag | Result |
|---|---|---|
| `OnCollisionEnter` | `Fance` | Plays `hit` animation only |
| `OnCollisionEnter` | `Obstacle` | `gm.ChangeLife(-1)`, plays `hit` animation, destroys the obstacle |
| `OnTriggerEnter` | `Coin` | `gm.ChangeCoin(10)`, destroys the coin |

---

### `GameManagement.cs`
Tracks score, coins, and life; drives the Game Over UI and scene transitions.

**Inspector Fields**

| Field | Type | Description |
|---|---|---|
| `scoreUI`, `scoreShow`, `CoinUI`, `lifeUI` | `TMP_Text` | HUD / results text fields |
| `red` | `Image` | Damage flash / vignette image |
| `GameOverUI`, `Show`, `retry` | `GameObject` | UI panels toggled on death |
| `player`, `level` | `GameObject` | Deactivated on death to freeze gameplay |
| `scoreFactor` | `float` | Multiplier applied to `Time.time` for scoring |

**Flow**
- `Update()` recalculates `score = scoreFactor * Time.time` every frame — a survival-time score, not a distance-based one.
- Life reaching `0` triggers `GameOver()`, which disables the player/level, shows the Game Over UI, and displays the final coin count.
- `ChangeCoin(int)` / `ChangeLife(int)` are the only public mutators, called from `PlayerCollision`.
- `Restart()` reloads scene `1`; `MainMenu()` loads scene `0`.

```csharp
private void Update()
{
    score = scoreFactor * Time.time;
    scoreUI.text = ((int)score).ToString();

    if (life <= 0)
    {
        GameOver();
    }
}
```

---

### `ObstaclesDestroy.cs`
A cleanup trigger volume (likely placed behind the camera) that despawns any obstacle that drifts past it.

```csharp
private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Obstacle"))
    {
        Destroy(other.gameObject);
    }
}
```

---

## 🎮 Controls

| Input | Action |
|---|---|
| Move (bound via Input System, e.g. WASD / Left Stick) | Strafe player between lanes / within clamp bounds |

> Movement is read through Unity's **Input System** package via a `PlayerInput` component calling `Movement.Move(InputAction.CallbackContext)` — bind the action asset's move control to your preferred device (keyboard, gamepad, or on-screen joystick).

---

## 🏷️ Tags Required

| Tag | Used By | Purpose |
|---|---|---|
| `Fance` | `PlayerCollision.cs` | Fence collision → hit animation |
| `Obstacle` | `PlayerCollision.cs`, `ObstaclesDestroy.cs` | Obstacle collision → life loss / cleanup |
| `Coin` | `PlayerCollision.cs` | Coin pickup → score increase |

---

## 🛠️ Setup & Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Chandan-Baskey/RoyalRun-3dGame.git
   ```
2. Open the project in **Unity 2022.3 LTS or later**.
3. Ensure the following packages are installed via Package Manager:
   - `TextMeshPro`
   - `Input System`
4. Add both the Main Menu scene and the Gameplay scene to **Build Settings**, in that order (index `0` = Main Menu, index `1` = Gameplay — see Known Issues).
5. On the player prefab, manually assign the `GameManagement` reference (`gm`) on `PlayerCollision.cs` in the Inspector.
6. Tag fence, obstacle, and coin prefabs with `Fance`, `Obstacle`, and `Coin` respectively.
7. Press Play from the Main Menu scene.

---

## 📁 Project Structure

```
Assets/
└── Scripts/
    ├── MainUI.cs             # Main menu Start/Quit buttons
    ├── LevelGenerator.cs     # Procedural chunk spawning & scrolling
    ├── Chunks.cs             # Per-chunk fence/coin lane placement
    ├── Swpawner.cs           # Secondary obstacle spawner
    ├── Movement.cs           # Player lane movement (Input System)
    ├── PlayerCollision.cs    # Collision → animation / life / coin events
    ├── GameManagement.cs     # Score, life, coin, Game Over flow
    └── ObstaclesDestroy.cs   # Off-screen obstacle cleanup trigger
```

---

## 🐞 Known Issues

| Issue | Location | Detail |
|---|---|---|
| Unused `NUnit.Framework` import | `Chunks.cs` | `using NUnit.Framework;` is present but nothing from it is used — an unnecessary editor/test-only dependency in a runtime script |
| `gm` reference not auto-resolved | `PlayerCollision.cs` | The `FindFirstObjectByType<GameManagement>()` call in `Start()` is commented out, so `gm` must be manually dragged into the Inspector or coin/life events will throw a `NullReferenceException` |
| Randomized spawn position unused | `Swpawner.cs` | `spawnPosition` is calculated using `Random.Range(-spawnWidth, spawnWidth)` but `Instantiate()` uses `transform.position` instead, so `spawnWidth` has no actual effect and every obstacle spawns at the same point |
| `numToSpawn` never enforced | `Swpawner.cs` | The coroutine decrements `numToSpawn` every loop but never checks it against `0`, so obstacles spawn forever regardless of the configured cap |
| Hardcoded scene indices | `MainUI.cs`, `GameManagement.cs` | `SceneManager.LoadScene(0)` / `LoadScene(1)` assume a fixed Build Settings order; reordering scenes silently breaks navigation |
| Coin lane bias | `Chunks.cs` | `SpawnCoin()` always uses `availableLance[0]`, so the coin's lane is not randomized among the remaining open lanes — it consistently favors the lowest-index lane left |
| Score is time-based, not distance-based | `GameManagement.cs` | `score = scoreFactor * Time.time` scales with `Time.time` (time since scene load), so pausing/idling still increases score even if the player isn't progressing |
| `Camera.main` lookup per chunk per frame | `LevelGenerator.cs` | `MoveChunk()` calls `Camera.main` inside a loop every frame; caching it once in `Start()`/`Awake()` would be more efficient |

---

## 🚧 Roadmap / Future Improvements

- [ ] Fix `Swpawner.cs` to actually use the randomized `spawnPosition` and respect `numToSpawn` as a real cap
- [ ] Auto-resolve `GameManagement` reference in `PlayerCollision.cs` (re-enable `FindFirstObjectByType`) or assign via a singleton
- [ ] Remove the stray `NUnit.Framework` import from `Chunks.cs`
- [ ] Replace hardcoded scene indices with named scene references or a `GameSceneManager` constant list
- [ ] Randomize coin lane selection instead of always defaulting to `availableLance[0]`
- [ ] Add a difficulty curve (increasing `moveSpeed` / spawn rate over time)
- [ ] Add sound effects and background music
- [ ] Add a pause menu
- [ ] Cache `Camera.main` reference to avoid repeated lookups in `LevelGenerator.cs`

---

## 📄 License

This project is licensed under the MIT License.
