!include nsDialogs.nsh
!include LogicLib.nsh
!include WinVer.nsh


; �ýű�ʹ�� HM VNISEdit �ű��༭���򵼲���

; ��װ�����ʼ���峣��

!define PRODUCT_NAME "2D�������ģ��"
!define PRODUCT_VERSION "V1.0"
!define PRODUCT_PUBLISHER "�Ϻ�����������Ϣ�������޹�˾"
!define PRODUCT_WEB_SITE "http://www.vadeware.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\Random_Polygon\Random_Polygon_CSharp.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"

SetCompressor lzma

; ------ MUI �ִ����涨�� (1.67 �汾���ϼ���) ------
!include "MUI.nsh"

; MUI Ԥ���峣��
!define MUI_ABORTWARNING
!define MUI_ICON "include\install.ico"
!define MUI_UNICON ".\include\Remove.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP ".\include\welcome.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP ".\include\Bannerstatic.bmp" ; optional

; ��ӭҳ��
!insertmacro MUI_PAGE_WELCOME

; ��װĿ¼ѡ��ҳ��
!insertmacro MUI_PAGE_DIRECTORY
; ��ʼ�˵�����ҳ��
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "�������ģ��"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP
; ��װ����ҳ��
!insertmacro MUI_PAGE_INSTFILES
; ��װ���ҳ��
!define MUI_FINISHPAGE_RUN "$INSTDIR\VadeFaxC.exe"
!insertmacro MUI_PAGE_FINISH

; ��װж�ع���ҳ��
!insertmacro MUI_UNPAGE_INSTFILES

; ��װ�����������������
!insertmacro MUI_LANGUAGE "SimpChinese"

; ��װԤ�ͷ��ļ�
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI �ִ����涨����� ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Standard.exe"
InstallDir "$PROGRAMFILES\VadeWare\Random_Polygon"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show

Section "�ͻ���" MAIN
  SectionIn RO   
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  # ���ð�װĿ¼$INSTDIR��Ȩ��
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


; ������ʼ�˵���ݷ�ʽ
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\2D�������ģ��.lnk" "$INSTDIR\Random_Polygon_CSharp.exe"
  CreateShortCut "$DESKTOP\2D�������ģ��.lnk" "$INSTDIR\Random_Polygon_CSharp.exe"
  InvokeShellVerb::DoIt "$DESKTOP" "2D�������ģ��.lnk" "5381"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd
 

Section -AdditionalIcons
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\Website.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\���ʹ�˾��ҳ.lnk" "$INSTDIR\Website.url"
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
 *  �����ǰ�װ�����ж�ز���  *
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
  Delete "$DESKTOP\VadeFax����ͨ�ͻ���.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\VadeFax����ͨ�ͻ���.lnk"

  RMDir /r "$SMPROGRAMS\$ICONS_GROUP"
  RMDir /r ""

  ;RMDir /r "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "��ȷʵҪ��ȫ�Ƴ�2D�������ģ�ͣ��������е������" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "2D�������ģ�� �ѳɹ��ش���ļ�����Ƴ���"
FunctionEnd

 
 
