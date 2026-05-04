# TODO

## Player Model
- [ ] Replace capsule placeholder with real player model
- [ ] Create AnimatorController with `LeftShoot` and `RightShoot` triggers
- [ ] Assign AnimatorController to Player's Animator component
- [ ] Wire `InputSystem → animator` field to Player's Animator

## Enemy Prefab
- [ ] Assign `AmmoPack` child GameObject to `Enemy → AmmoPack` field in Inspector
- [ ] Assign `Animator` with triggers: `IsAttack`, `IsDead`
- [ ] Assign `deathParticle` ParticleSystem

## Inspector (manual assigns remaining)
- [ ] `EnemySpawner → enemyPrefabs[]` — drag `Prefabs/Enemy/Enemy.prefab`
- [ ] `AudioManager` — assign all SFX clips from `Audio/SFX/`
- [ ] `UIManager` — wire all buttons (start, difficulty, game over, level complete, etc.)
