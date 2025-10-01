En esta carpeta muestro fragmentos completos de código de mi juego **FPS postapocalíptico** en Unity.  
Cada script resalta una parte distinta del desarrollo: IA, gameplay, UI, economía y arquitectura.

---

## 🤖 Enemy AI (State Machine)
- **Scripts:** `StateMachine.cs`, `BaseState.cs`, `PatrolState.cs`, `AttackState.cs`
- Implementación de una **máquina de estados** para controlar el comportamiento de los enemigos.
- Incluye lógica de patrulla, persecución y ataque con cooldowns y animaciones.

---

## 🧟 Enemy.cs
- Control de vida, animaciones, muerte y **drop aleatorio de monedas**.
- Respawn automático mediante `EnemyRespawner`.

---

## 🎮 PlayerMovement.cs
- Movimiento en primera persona con **sprint, estamina, gravedad y control de cámara**.
- Actualización de la UI de estamina.

---

## 🔫 SimplePistol.cs
- Sistema de disparo con **raycast y precisión dinámica**.
- Gestión de munición y actualización de la UI.

---

## 🛒 ShopManager.cs
- **Tienda in-game**: permite comprar vida, munición y desbloquear armas.
- Control del tiempo (`Time.timeScale`) y del cursor en menús.
