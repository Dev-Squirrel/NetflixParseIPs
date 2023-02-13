# NetflixParseIPs

## 어떤 프로그램인가요?
OpenVPN Netflix Routing 설정에 필요한 IP 파싱 프로그램입니다.  
2023년에 적용될 동거인 외 계정공유를 검증하는 방법으로 31일에 한 번씩 기본값으로 설정한 무선 인터넷에 연결하는 방법을 우회하려고 만들었습니다.  

## 사용 방법은 무엇인가요?
NetflixParseIPs 프로그램 실행하시면 자동으로 IP 추출 후 result.txt 파일이 생성됩니다.  
OpenVPN 클라이언트 설정 파일에 추가하시고 사용하시면 됩니다.  
자세한 사용 방법은 OpenVPN 사이트를 참조하시기를 바랍니다.  

## IP 값은 어디서 가져오나요?
IPinfo.io 사이트에서 가져옵니다.  
https://ipinfo.io/AS2906

## 참조 사이트
airvpn.org zqwvyx님 - 올려주신 코드 참조하여 작업하였습니다.  
https://airvpn.org/forums/topic/19781-bypass-vpn-for-specific-domain-names-netflix-hulu-via-custom-configuration-in-openvpn-tomato-dd-wrtrouter/?tab=comments#comment-71268  
  
ChatGPT - C# 코드 변환 및 일부 코드는 ChatGPT 이용하여 작성했습니다.  
https://openai.com/  
  
Google - 구글님은 답을 알고 있다  
https://google.com

## 주의!!!
프로그램 사용으로 인한 발생하는 문제에 대한 모든 책임은 사용자에게 있습니다.  
API 이용이 아닌 HtmlAgilityPack 사용으로 가져오므로 IP 차단 가능성이 있으니 주의 부탁드립니다.

## LICENSE
MIT License  
  
Copyright (c) 2023 Dev-Squirrel