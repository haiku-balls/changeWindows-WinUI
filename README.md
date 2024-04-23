![](https://github.com/haiku-balls/changeWindows-WinUI/blob/master/Assets/haikuChangeWindowsBanner.png)
<div align="center">
  <h3>Change Windows WinUI</h3>
  <p>Track Windows Installs!</p>
</div>

---

> [!NOTE]
> This program is really rough; while the code might work, it's not good by any means. ;-)

## ❓What?
This program tracks your windows installs. Similar to the built in one from Windows Update, but it logs more information about it. (Such as what insider channel it came from or its full build tag)
This program is catered towards Windows Insiders who install builds very often. 

## ⁉️Why?
As an Insider since '19, I always disliked that there isn't any log that gives me lots of info about the builds I've flighted in the past. I created this program mainly for myself (and others) who share the same grievances. The Windows Update log is rather lackluster as it only tells you what build it was and the date.

## ✨Feature Set
- Captures new builds and their related information from the registry.
- Logs builds to a xml file.

## 🚧 Current Problems
*Have a solution to these problems? Help me out and make a [pull request.](https://github.com/haiku-balls/changeWindows-WinUI/pulls) Anyone is welcome to make one.*
- ~~The install date is taken from the date you FIRST installed windows, not when a build was installed.~~
- The branch listings under builds are captured depending on what insider channel you're enrolled in, not the build's channel.

## Preview
![](https://github.com/haiku-balls/changeWindows-WinUI/blob/master/Assets/changeWindowsPreview.png)
