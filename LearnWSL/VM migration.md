# VM Migraration 

한 장비에서 구성한 대로 다른 장비로 이동하는 방법을 다룬다. 

https://stackoverflow.com/questions/38779801/move-wsl-bash-on-windows-root-filesystem-to-another-hard-drive

- 여기 설명이 자세하고 정확하여 동작할 것으로 보인다. 


현재 설치된 배포 버전 확인 

- F:\> wsl --list 

익스포트 

- F:\> wsl --export CentOS7 CentOs7.tar
 
임포트 

- wsl.exe --import <DistributionName> <Folder-To-Install> <Tar-FileName>

export에서 멈추는 듯 하다가 다시 하니 완료했다. 


