ScaleFree: Dynamic KDE for Multiscale Point Cloud Exploration in VR
======

|![ScaleFree_Image](https://raw.githubusercontent.com/LixiangZhao98/asset/master/Project/ScaleFree/pic/teaser.png "ScaleFree_Image") |
|:--------------------------------------------------------------------------------------------------------------------------:|

[Arxiv](https://arxiv.org/abs/2407.14833) | [Video(coming soon)]( "Video") | [Paper(coming soon)]("Paper")

ScaleFree is a a GPU-accelerated adaptive Kernel Density Estimation (KDE) algorithm for scalable, interactive multiscale point cloud exploration. Refer to the [Video(coming soon)]( "Video") for a quick review. For more details, please refer to our [Paper(coming soon)]("Paper") (ScaleFree: Dynamic KDE for Multiscale Point Cloud Exploration in VR), which will be presented on [IEEE VR 2026](https://ieeevr.org/2026/ "VR2026").\
Any pull requests and issues are welcome. If you find it useful, could you please give a star? Thanks in advance.

# Requirements
- Windows platform
- Unity 3D (version>2019)

# Install the project and Play the demo
- Download Unity Hub. Please refer to sec.1-4 in [tutorial](https://github.com/LixiangZhao98/asset/blob/master/Tutorial/Unity_Setup_General.pdf) if you are a new Unity user.
- Clone the repo or download the archive [https://github.com/ScaleFree/archive/refs/heads/master.zip](https://github.com/LixiangZhao98/ScaleFree/archive/refs/heads/master.zip "archive") and open the project using Unity (versions equal to/higher than 2019 have been tested). Please refer to sec.6 in [tutorial](https://github.com/LixiangZhao98/asset/blob/master/Tutorial/Unity_Setup_General.pdf) if you don't know how to open an existing project.
- `Assets/PointCloud-Visualization-Tool/scenes/KernelDensityEstimation.unity` is the demo scene.

# Control
- Press `Start` and you can see the real-time iso-surface reconstruction of the density estimation (Marching Cubes).
- To switch the dataset, click the DataObject in hierarchy, click `DataLoader` child gameObject and change variable `datasets in project` in the inspector window. 
- To change MarchingCube threshold, click the DataObject in hierarchy, click `MarchingCube` child gameObject and adjust the variable `MC Threshold` in the inspector window.

![Image](https://github.com/LixiangZhao98/asset/blob/master/Project/ScaleFree/pic/demo.png "Image")



## BibTex