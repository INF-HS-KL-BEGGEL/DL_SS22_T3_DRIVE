# Unity-Environement zum Reinforcement Learning von selbstfahrenden Autos

Innerhalb der Projektzeit wurde ein Environement zum Trainieren von selbstfahrenden Autos mittels des Reinforcement Learnings entwickelt.

## Benötigte Installation von Software
| Unity | 2021.3.1f1 LTS |
| Python | 3.7+ |
| Unity ML Agents Python Package | 0.28.0 |
| PyTorch | 1.7.1 |

Unity ist kostenlos für Schüler und Studierende ab 16 Jahren. Dazu muss man sich mit der Studentenmail und SheerID [registrieren](https://store.unity.com/de/#plans-individual).

Python kann unter [python.org](https://www.python.org/downloads/release/python-370/) herutnergeladen werden.

```
python -m pip install --upgrade pip
pip install TODO
pip install TODO
```
Alternativ kann PyTorch auf der [offiziellen Website](https://pytorch.org/) heruntergeladen werden.

## Abhängigkeiten des Projektes
Diese sollten vom *Unity Package Manager* richtig eingetragen werden und entsprechende Pakete installiert werden.
```JSON
{
  "dependencies": {
    "com.unity.burst": "1.8.0-pre.1",
    "com.unity.collab-proxy": "1.15.18",
    "com.unity.collections": "1.2.3",
    "com.unity.ide.rider": "3.0.14",
    "com.unity.ide.visualstudio": "2.0.15",
    "com.unity.ide.vscode": "1.2.5",
    "com.unity.jobs": "0.50.0-preview.9",
    "com.unity.ml-agents": "2.0.1",
    "com.unity.test-framework": "1.1.31",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.timeline": "1.6.4",
    "com.unity.ugui": "1.0.0",
    "com.unity.visualscripting": "1.7.8",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}
```

## Config.yaml zur Konfiguration 
Vergleiche mit ([offiziellem Repository](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-Configuration-File.md))

```yaml
default_settings:
    trainer_type: ppo
    hyperparameters:
      batch_size: 2048
      beta: 5.0e-3
      buffer_size: 20480
      epsilon: 0.2
      num_epoch: 10000
      lambd: 0.90
      learning_rate: 3.0e-4
      learning_rate_schedule: linear
    network_settings:
      normalize: false
      hidden_units: 128
      vis_encode_type: simple
      num_layers: 2
      memory:
        sequence_length: 64
        memory_size: 128
      use_recurrent: false
    max_steps: 5.0e5
    time_horizon: 64
    summary_freq: 1000
    reward_signals:
        extrinsic:
            strength: 1.0
            gamma: 0.99

behaviors:
  rl_deep_race:
    network_settings:
      normalize: true
      num_layers: 3
      hidden_units: 512
    hyperparameters:
      batch_size: 2048
      buffer_size: 20480
    time_horizon: 1000
    max_steps: 2e6
    summary_freq: 3000
    keep_checkpoints: 5
```

### Mögliche Beobachtungen des Agents
*RLParameters.cs*
```C#
// TODO:
```

### Rewardfunction
*RLDriver.cs*
```C#
// TODO:
```

### SessionHolder
*SessionHolder.cs*
// TODO:

### Training

Commands zum Starten
// TODO --force
-- resume
tensor board

<img src="../res/2_Unity_ML_Agents_Tensorboard.png" align="middle" width="720"/> 


## Über die Technik ([Doku](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Unity-Inference-Engine.md))

- Extern trainierte Models werden offiziell nicht unterstützt
- Externe Trainer über Python Low-Level API andockbar 
- Tensorflow wurde von PyTorch seit [Release 13](https://github.com/Unity-Technologies/ml-agents/releases/tag/release_13) als Trainer abgelöst, da es keine native C# Unterstützung bietet
- Es ist aber möglich wenn die Input und Output Tensoren angepasst werden
- Grund ist eigene Engine die auf Compute Shader basiert
- Baut auf Pytorch und dem Open Neural Network Exchange Format auf