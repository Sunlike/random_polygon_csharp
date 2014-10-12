!include nsDialogs.nsh
!include LogicLib.nsh
!include WinVer.nsh


; 该脚本使用 HM VNISEdit 脚本编辑器向导产生

; 安装程序初始定义常量

!define PRODUCT_NAME "2D骨料填充模型"
!define PRODUCT_VERSION "V1.0"
!define PRODUCT_PUBLISHER "上海卫德卫尔信息技术有限公司"
!define PRODUCT_WEB_SITE "http://www.vadeware.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\Random_Polygon\Random_Polygon_CSharp.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"

SetCompressor lzma

; ------ MUI 现代界面定义 (1.67 版本以上兼容) ------
!include "MUI.nsh"

; MUI 预定义常量
!define MUI_ABORTWARNING
!define MUI_ICON "include\install.ico"
!define MUI_UNICON ".\include\Remove.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP ".\include\welcome.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP ".\include\Bannerstatic.bmp" ; optional

; 欢迎页面
!insertmacro MUI_PAGE_WELCOME

; 安装目录选择页面
!insertmacro MUI_PAGE_DIRECTORY
; 开始菜单设置页面
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "骨料填充模型"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP
; 安装过程页面
!insertmacro MUI_PAGE_INSTFILES
; 安装完成页面
!define MUI_FINISHPAGE_RUN "$INSTDIR\VadeFaxC.exe"
!insertmacro MUI_PAGE_FINISH

; 安装卸载过程页面
!insertmacro MUI_UNPAGE_INSTFILES

; 安装界面包含的语言设置
!insertmacro MUI_LANGUAGE "SimpChinese"

; 安装预释放文件
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI 现代界面定义结束 ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Standard.exe"
InstallDir "$PROGRAMFILES\VadeWare\Random_Polygon"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show

Section "客户端" MAIN
  SectionIn RO   
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  # 设置安装目录$INSTDIR的权限
  # See http://support.microsoft.com/kb/163846 for SIDs 
  # Everyone
  AccessControl::GrantOnFile "$INSTDIR" "(S-1-1-0)" "FullAccess"
  # Users
  AccessControl::GrantOnFile "$INSTDIR" "(S-1-5-32-545)" "FullAccess"
  
  File "Release\acdbmgd.dll"
  File "Release\CadHelper.dll"
  File "Release\CadHelper.pdb"
  File "Release\Random_Polygon_CSharp.exe"
  File "Release\Random_Polygon_CSharp.pdb"
  File "Release\Random_Polygon_CSharp.vshost.exe"
  File "Release\Random_Polygon_CSharp.vshost.exe.manifest"
  
  
    
  SetOutPath "$INSTDIR\zh-CN"
  File "Release\zh-CN\acdbmgd.resources.dll"


; 创建开始菜单快捷方式
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\2D骨料填充模型.lnk" "$INSTDIR\Random_Polygon_CSharp.exe"
  CreateShortCut "$DESKTOP\2D骨料填充模型.lnk" "$INSTDIR\Random_Polygon_CSharp.exe"
  InvokeShellVerb::DoIt "$DESKTOP" "2D骨料填充模型.lnk" "5381"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd
 

Section -AdditionalIcons
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\Website.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\访问公司主页.lnk" "$INSTDIR\Website.url"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk" "$INSTDIR\uninst.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Random_Polygon_CSharp.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Random_Polygon_CSharp.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"

 
SectionEnd

/******************************
 *  以下是安装程序的卸载部分  *
 ******************************/

Section Uninstall
  !insertmacro MUI_STARTMENU_GETFOLDER "Application" $ICONS_GROUP
  ;UnRegister msjet40.dll and dao360.dll
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\acdbmgd.dll"
  Delete "$INSTDIR\CadHelper.dll"
  Delete "$INSTDIR\CadHelper.pdb"
  Delete "$INSTDIR\Random_Polygon_CSharp.exe"
  Delete "$INSTDIR\Random_Polygon_CSharp.pdb"
  Delete "$INSTDIR\Random_Polygon_CSharp.vshost.exe"
  Delete "$INSTDIR\Random_Polygon_CSharp.vshost.exe.manifest"
  Delete "$INSTDIR\zh-CN\acdbmgd.resources.dll"
  ;RMDir /r "$INSTDIR\users"
  RMDir /r "$INSTDIR\zh-CN"

  Delete "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\Website.lnk"
  Delete "$DESKTOP.lnk"
  Delete "$ICONS_GROUP.lnk"
  Delete "$DESKTOP\VadeFax传真通客户端.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\VadeFax传真通客户端.lnk"

  RMDir /r "$SMPROGRAMS\$ICONS_GROUP"
  RMDir /r ""

  ;RMDir /r "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

#-- 根据 NSIS 脚本编辑规则，所有 Function 区段必须放置在 Section 区段之后编写，以避免安装程序出现未可预知的问题。--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "你确实要完全移除2D骨料填充模型，及其所有的组件？" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "2D骨料填充模型 已成功地从你的计算机移除。"
FunctionEnd

 
 
