using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace FPS
{
    public static class GameBootstrap
    {
        private enum LevelTheme { Urban, Forest, Dungeon }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Run()
        {
            // Если уровень уже собран заранее (см. LevelBaker), объекты уже лежат в сцене -
            // достаточно только пересчитать NavMesh: он не сохраняется в сцену и нужен каждый запуск.
            if (Object.FindFirstObjectByType<BakedLevelMarker>() != null)
            {
                BakeNavMesh();
                return;
            }

            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case "FPS_Urban": BuildLevel(LevelTheme.Urban); break;
                case "FPS_Forest": BuildLevel(LevelTheme.Forest); break;
                case "FPS_Dungeon": BuildLevel(LevelTheme.Dungeon); break;
            }
        }

        // Точка входа для LevelBaker: собирает уровень в редакторе (вне Play Mode) и
        // помечает сцену как готовую, чтобы Run() больше не пересобирал её с нуля.
        public static void BakeLevelInEditor(string sceneName)
        {
            LevelTheme theme = sceneName switch
            {
                "FPS_Urban" => LevelTheme.Urban,
                "FPS_Forest" => LevelTheme.Forest,
                "FPS_Dungeon" => LevelTheme.Dungeon,
                _ => throw new System.ArgumentException($"Неизвестная сцена FPS: {sceneName}"),
            };

            BuildLevel(theme);
            new GameObject("BakedLevelMarker").AddComponent<BakedLevelMarker>();
        }

        private static void BuildLevel(LevelTheme theme)
        {
            RemoveStaticSceneCamera();
            BuildGround(theme);
            BuildDressing(theme);
            BuildCoverZone();
            BuildDestructible(new Vector3(4f, 0.5f, 2f));
            BuildDoor(new Vector3(-6f, 1.5f, 9f));
            BuildTrap(new Vector3(2f, 0.05f, -4f));
            ConfigureSun(theme);
            BakeNavMesh();

            GameObject player = BuildPlayer(new Vector3(0f, 1f, -6f));
            BuildEnemies(theme, player.transform);
            BuildTurret(new Vector3(7f, 1f, -7f));

            var minimap = new GameObject("MinimapSetup").AddComponent<MinimapSetup>();
            minimap.SetFollowTarget(player.transform);

            ControlsHud.Build();
        }

        private static void RemoveStaticSceneCamera()
        {
            foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                Object.Destroy(camera.gameObject);
            }
        }

        private static void BuildGround(LevelTheme theme)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(60f, 1f, 60f);
            ground.GetComponent<Renderer>().material.color = GroundColor(theme);
            ground.AddComponent<SurfaceMarker>().SetSurface(theme == LevelTheme.Forest ? SurfaceType.Sand : SurfaceType.Stone);
        }

        private static Color GroundColor(LevelTheme theme) => theme switch
        {
            LevelTheme.Forest => new Color(0.27f, 0.42f, 0.2f),
            LevelTheme.Dungeon => new Color(0.15f, 0.15f, 0.17f),
            _ => new Color(0.55f, 0.55f, 0.58f),
        };

        private static void BuildDressing(LevelTheme theme)
        {
            switch (theme)
            {
                case LevelTheme.Urban: BuildUrbanDressing(); break;
                case LevelTheme.Forest: BuildForestDressing(); break;
                case LevelTheme.Dungeon: BuildDungeonDressing(); break;
            }
        }

        private static void BuildUrbanDressing()
        {
            GameObject boxPrefab = ProjectAsset.Load<GameObject>(
                "Assets/StarterAssets/Environment/Prefabs/Box_350x250x200_Prefab.prefab");
            GameObject wallPrefab = ProjectAsset.Load<GameObject>(
                "Assets/StarterAssets/Environment/Prefabs/Wall_Prefab.prefab");

            Vector3[] boxSpots = { new(6f, 0f, 4f), new(-8f, 0f, -2f), new(3f, 0f, -10f), new(-4f, 0f, 4f) };
            foreach (Vector3 spot in boxSpots)
            {
                SpawnDressing(boxPrefab, spot, SurfaceType.Metal);
            }

            Vector3[] wallSpots = { new(14f, 0f, 0f), new(-14f, 0f, 0f), new(0f, 0f, 16f), new(0f, 0f, -16f) };
            foreach (Vector3 spot in wallSpots)
            {
                SpawnDressing(wallPrefab, spot, SurfaceType.Stone);
            }
        }

        private static void BuildForestDressing()
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 22f;
                BuildTree(new Vector3(offset.x, 0f, offset.y));
            }
        }

        private static void BuildTree(Vector3 position)
        {
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "TreeTrunk";
            trunk.transform.position = position + Vector3.up * 1.5f;
            trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
            trunk.GetComponent<Renderer>().material.color = new Color(0.36f, 0.25f, 0.15f);
            trunk.AddComponent<SurfaceMarker>().SetSurface(SurfaceType.Wood);

            var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "TreeCanopy";
            canopy.transform.position = position + Vector3.up * 4f;
            canopy.transform.localScale = new Vector3(2.6f, 2.2f, 2.6f);
            canopy.GetComponent<Renderer>().material.color = new Color(0.16f, 0.4f, 0.18f);
            canopy.AddComponent<SurfaceMarker>().SetSurface(SurfaceType.Wood);

            BuildCanopyLobe(position + new Vector3(0.9f, 3.3f, 0.4f), 1.6f, new Color(0.19f, 0.44f, 0.2f));
            BuildCanopyLobe(position + new Vector3(-0.8f, 3.6f, -0.5f), 1.5f, new Color(0.14f, 0.36f, 0.16f));
        }

        private static void BuildCanopyLobe(Vector3 position, float scale, Color color)
        {
            var lobe = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lobe.name = "TreeCanopyLobe";
            lobe.transform.position = position;
            lobe.transform.localScale = new Vector3(scale, scale * 0.85f, scale);
            lobe.GetComponent<Renderer>().material.color = color;
            lobe.AddComponent<SurfaceMarker>().SetSurface(SurfaceType.Wood);
        }

        private static void BuildDungeonDressing()
        {
            Vector3[] wallSpots = { new(10f, 1.5f, 0f), new(-10f, 1.5f, 0f), new(0f, 1.5f, 12f), new(0f, 1.5f, -12f) };
            foreach (Vector3 spot in wallSpots)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "DungeonWall";
                wall.transform.position = spot;
                bool horizontal = Mathf.Abs(spot.x) > Mathf.Abs(spot.z);
                wall.transform.localScale = horizontal ? new Vector3(1f, 3f, 24f) : new Vector3(24f, 3f, 1f);
                wall.GetComponent<Renderer>().material.color = new Color(0.2f, 0.18f, 0.2f);
                wall.AddComponent<SurfaceMarker>().SetSurface(SurfaceType.Stone);
            }

            Vector3[] torchSpots = { new(6f, 2f, 6f), new(-6f, 2f, -6f) };
            foreach (Vector3 spot in torchSpots)
            {
                var torch = new GameObject("Torch");
                torch.transform.position = spot;
                var light = torch.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(1f, 0.6f, 0.3f);
                light.intensity = 2.5f;
                light.range = 8f;
            }
        }

        private static void SpawnDressing(GameObject prefab, Vector3 position, SurfaceType surface)
        {
            if (prefab == null) return;

            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            MaterialCompat.FixForBuiltinPipeline(instance);
            if (instance.GetComponentInChildren<SurfaceMarker>() == null)
            {
                instance.AddComponent<SurfaceMarker>().SetSurface(surface);
            }
        }

        private static void BuildCoverZone()
        {
            var zoneObject = new GameObject("NoEnemyZone_Cover");
            zoneObject.transform.position = new Vector3(-3f, 1f, -3f);
            zoneObject.AddComponent<NoEnemyZone>();

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "CoverMarker";
            visual.transform.SetParent(zoneObject.transform, false);
            visual.transform.localScale = new Vector3(6f, 0.05f, 6f);
            visual.GetComponent<Collider>().enabled = false;
            visual.GetComponent<Renderer>().material.color = new Color(0.2f, 0.6f, 1f, 0.4f);
        }

        private static void BuildDestructible(Vector3 position)
        {
            GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crate.name = "DestructibleCrate";
            crate.transform.position = position;
            crate.transform.localScale = Vector3.one;
            crate.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.15f);
            crate.AddComponent<SurfaceMarker>().SetSurface(SurfaceType.Wood);
            crate.AddComponent<Destructible>();

            Color strapColor = new Color(0.3f, 0.2f, 0.08f);
            AddDecoration(crate.transform, PrimitiveType.Cube, Vector3.zero, new Vector3(1.05f, 0.08f, 0.15f), strapColor);
            AddDecoration(crate.transform, PrimitiveType.Cube, Vector3.zero, new Vector3(0.15f, 0.08f, 1.05f), strapColor);
        }

        private static void AddDecoration(Transform parent, PrimitiveType type, Vector3 localPos, Vector3 localScale, Color color, Quaternion? localRot = null)
        {
            GameObject decoration = GameObject.CreatePrimitive(type);
            decoration.name = "Decoration";
            decoration.transform.SetParent(parent, false);
            decoration.transform.localPosition = localPos;
            decoration.transform.localRotation = localRot ?? Quaternion.identity;
            decoration.transform.localScale = localScale;
            decoration.GetComponent<Renderer>().material.color = color;
            Object.Destroy(decoration.GetComponent<Collider>());
        }

        private static void BuildDoor(Vector3 position)
        {
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Door";
            door.transform.position = position;
            door.transform.localScale = new Vector3(3f, 3f, 0.2f);
            door.GetComponent<Renderer>().material.color = new Color(0.4f, 0.28f, 0.15f);
            door.GetComponent<Collider>().isTrigger = false;

            var trigger = new GameObject("DoorTrigger");
            trigger.transform.position = position;
            BoxCollider triggerCollider = trigger.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(4f, 4f, 3f);
            trigger.transform.SetParent(door.transform, true);

            door.AddComponent<Door>();

            var handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            handle.name = "DoorHandle";
            handle.transform.position = position + new Vector3(1.2f, -0.3f, 0.15f);
            handle.transform.localScale = Vector3.one * 0.15f;
            handle.GetComponent<Renderer>().material.color = new Color(0.7f, 0.65f, 0.2f);
            Object.Destroy(handle.GetComponent<Collider>());
            handle.transform.SetParent(door.transform, true);
        }

        private static void BuildTrap(Vector3 position)
        {
            GameObject trap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trap.name = "Trap";
            trap.transform.position = position;
            trap.transform.localScale = new Vector3(2f, 0.1f, 2f);
            trap.GetComponent<Renderer>().material.color = Color.red;
            trap.GetComponent<Collider>().isTrigger = true;
            trap.AddComponent<Trap>();

            Color spikeColor = new Color(0.25f, 0.25f, 0.27f);
            Vector3[] spikeOffsets = { new(-0.5f, 0.12f, -0.5f), new(0.5f, 0.12f, -0.5f), new(-0.5f, 0.12f, 0.5f), new(0.5f, 0.12f, 0.5f), new(0f, 0.15f, 0f) };
            foreach (Vector3 offset in spikeOffsets)
            {
                var spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spike.name = "TrapSpike";
                spike.transform.position = position + offset;
                spike.transform.localScale = new Vector3(0.08f, 0.15f, 0.08f);
                spike.GetComponent<Renderer>().material.color = spikeColor;
                Object.Destroy(spike.GetComponent<Collider>());
            }
        }

        private static void ConfigureSun(LevelTheme theme)
        {
            Light sun = null;
            foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (light.type == LightType.Directional)
                {
                    sun = light;
                    break;
                }
            }
            if (sun == null) return;

            var cycle = sun.gameObject.AddComponent<DayNightCycle>();
            LightingMode mode = theme switch
            {
                LevelTheme.Dungeon => LightingMode.Night,
                LevelTheme.Forest => LightingMode.Dynamic,
                _ => LightingMode.Day,
            };
            cycle.SetMode(mode);
        }

        private static void BakeNavMesh()
        {
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            var bounds = new Bounds(Vector3.zero, new Vector3(90f, 20f, 90f));

            NavMeshBuilder.CollectSources(bounds, ~0, NavMeshCollectGeometry.PhysicsColliders, 0, markups, sources);

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
            NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds, Vector3.zero, Quaternion.identity);
            NavMesh.AddNavMeshData(data);
        }

        private static GameObject BuildPlayer(Vector3 position)
        {
            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = position;

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.radius = 0.35f;

            var cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.SetParent(player.transform, false);
            cameraPivot.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            var cameraObject = new GameObject("PlayerCamera");
            cameraObject.transform.SetParent(cameraPivot.transform, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.nearClipPlane = 0.05f;

            var fpsController = player.AddComponent<FpsPlayerController>();
            fpsController.SetCameraPivot(cameraPivot.transform);

            player.AddComponent<PlayerHealth>();
            player.AddComponent<FootstepPlayer>();

            var weaponHolder = new GameObject("WeaponHolder");
            weaponHolder.transform.SetParent(cameraObject.transform, false);
            weaponHolder.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);

            WeaponController[] weapons = BuildWeaponSet(weaponHolder.transform, camera);
            player.AddComponent<WeaponSwitcher>().SetWeapons(weapons);

            return player;
        }

        private static WeaponController[] BuildWeaponSet(Transform holder, Camera aimCamera)
        {
            string[] assetPaths =
            {
                "Assets/GameData/Weapons/Pistol.asset",
                "Assets/GameData/Weapons/SMG.asset",
                "Assets/GameData/Weapons/Shotgun.asset",
                "Assets/GameData/Weapons/Sniper.asset",
                "Assets/GameData/Weapons/RocketLauncher.asset",
            };

            var result = new WeaponController[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                WeaponData data = ProjectAsset.Load<WeaponData>(assetPaths[i]);

                var weaponObject = new GameObject(data != null ? data.weaponName : $"Weapon_{i}");
                weaponObject.transform.SetParent(holder, false);

                var muzzle = new GameObject("Muzzle");
                muzzle.transform.SetParent(weaponObject.transform, false);
                muzzle.transform.localPosition = new Vector3(0f, 0f, 0.6f);

                WeaponController weapon = weaponObject.AddComponent<WeaponController>();
                weapon.Configure(data, muzzle.transform, aimCamera, Faction.Player);
                WeaponVisual.Build(weaponObject.transform, data);
                result[i] = weapon;
            }

            return result;
        }

        private static void BuildEnemies(LevelTheme theme, Transform player)
        {
            Vector3[] rangedSpots = { new(8f, 0f, 6f), new(-8f, 0f, 9f), new(6f, 0f, -6f) };
            foreach (Vector3 spot in rangedSpots)
            {
                BuildRangedEnemy(spot);
            }

            BuildMeleeEnemy(new Vector3(-6f, 0f, -8f));
        }

        private static GameObject CreateEnemyRoot(string name, Vector3 position)
        {
            var root = new GameObject(name);
            root.transform.position = position;

            NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
            agent.radius = 0.4f;
            agent.height = 1.9f;
            agent.speed = 3.2f;
            agent.acceleration = 12f;

            return root;
        }

        private static void BuildRangedEnemy(Vector3 position)
        {
            GameObject root = CreateEnemyRoot("EnemyRanged", position);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            body.GetComponent<Renderer>().material.color = new Color(0.7f, 0.15f, 0.15f);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            head.transform.localScale = Vector3.one * 0.35f;
            head.GetComponent<Renderer>().material.color = new Color(0.6f, 0.12f, 0.12f);

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(root.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 1.2f, 0.6f);

            WeaponData data = ProjectAsset.Load<WeaponData>("Assets/GameData/Weapons/SMG.asset");
            WeaponController weapon = root.AddComponent<WeaponController>();
            weapon.Configure(data, muzzle.transform, null, Faction.Enemy);

            root.AddComponent<RangedAttack>();
            root.AddComponent<EnemyBase>();
        }

        private static void BuildMeleeEnemy(Vector3 position)
        {
            GameObject root = CreateEnemyRoot("EnemyMelee", position);

            GameObject heroPrefab = ProjectAsset.Load<GameObject>(FpsAssetPaths.RpgHeroPrefab);
            if (heroPrefab != null)
            {
                GameObject visual = Object.Instantiate(heroPrefab, root.transform, false);
                visual.transform.localPosition = Vector3.zero;
                MaterialCompat.FixForBuiltinPipeline(visual);
            }

            var collider = root.AddComponent<CapsuleCollider>();
            collider.height = 1.9f;
            collider.radius = 0.4f;
            collider.center = new Vector3(0f, 0.95f, 0f);

            root.AddComponent<MeleeAttack>();
            root.AddComponent<EnemyBase>();
        }

        private static void BuildTurret(Vector3 position)
        {
            var turretObject = new GameObject("AutoTurret");
            turretObject.transform.position = position;

            var baseVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseVisual.name = "TurretBase";
            baseVisual.transform.SetParent(turretObject.transform, false);
            baseVisual.transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
            baseVisual.GetComponent<Renderer>().material.color = Color.gray;

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.name = "TurretBarrel";
            barrel.transform.SetParent(turretObject.transform, false);
            barrel.transform.localPosition = new Vector3(0f, 0.6f, 0.4f);
            barrel.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            barrel.GetComponent<Renderer>().material.color = Color.black;

            var sensor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sensor.name = "TurretSensor";
            sensor.transform.SetParent(turretObject.transform, false);
            sensor.transform.localPosition = new Vector3(0f, 0.85f, 0.35f);
            sensor.transform.localScale = Vector3.one * 0.15f;
            sensor.GetComponent<Renderer>().material.color = new Color(0.8f, 0.1f, 0.1f);
            Object.Destroy(sensor.GetComponent<Collider>());

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(turretObject.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0.6f, 0.9f);

            WeaponData data = ProjectAsset.Load<WeaponData>("Assets/GameData/Weapons/Pistol.asset");
            WeaponController weapon = turretObject.AddComponent<WeaponController>();
            weapon.Configure(data, muzzle.transform, null, Faction.Player);

            turretObject.AddComponent<AutoTurret>();
        }
    }
}
