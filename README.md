# DeCode
Currently built in Unity 2019.1.3f1, the stable version as of 2019-05-19. Open and load as usual with any Unity project.

## Assets
This project includes assets from the [Kenney Asset Pack](https://www.kenney.nl/assets) and [Ultimate Isometric Toolkit](https://assetstore.unity.com/packages/tools/sprite-management/isometric-toolkit-33032) by CodeBeans. The latter technically requires a license.

## Build Support
Builds have been tested to be working on the following platforms;
* Android
* WebGL
* Windows Desktop

We expect that DeCode should be buildable for any platform.

## Analytics Settings
The analytics endpoint is defined in the `LevelSwitchStatisticsManager` class, found in `Assets/Scripts/Misc/`. Edit the constants `ApiEndpoint` and `UserApiEndpoint` to change the endpoint. See the documents for the server code for more information.

## Misc.
* Many levels use Aspect ratio fit scripts, which means opening the level in the editor will "change" the dimensions for the given level. Be aware of this in order to not commit unneeded changes to the repo
* Git LFS is used for large media files