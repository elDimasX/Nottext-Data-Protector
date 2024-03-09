# Nottext Data Protector
Projeto em C# e Kernel que faz com que proteja, bloqueie, oculte e várias outras opções para suas pastas ou arquivos, serve como um anti-ransomware, proteger arquivos ou sua privacidade (fazia muito tempo que eu queria fazer esse programa kk)<br/>
Ele funciona como um programa portátil e funciona desde o Windows 7 até Windows 11, de 32 a 64 bits<br/>
Ele é extremamente fácil de modificar e possui o código todo comentado, portanto, creio que seja de fácil compreensão, mas se você tive dúvidas, me envie uma mensagem no Discord: eldimas

## Como funciona
O Nottext Data Protector funciona ativando o modo de teste do Windows (não tenho certificado digital) e instalando um driver que faz uma comunicação básica entre arquivos compartilhados por eles dois<br/>

Aqui está um exemplo <br/>
![Captura de tela 2024-03-09 150426](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/0e15e2ca-ef3d-47e4-966e-0ff5f82bda3d)


 <br/><br/>
E aqui em baixo segue algumas fotos dele <br/>

![2024-03-09](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/b54830f4-b0ea-4c7c-ab64-ca2d9aafd2bf)
![2024-03-09 (1)](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/35d46654-9024-4040-a583-b6ab5eff8e6b)
![2024-03-09 (2)](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/febfa45b-37f4-402b-b6cb-ac89aaf0c78f)
![2024-03-09 (3)](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/4a67c5fb-b21b-47dd-8574-a67e062e4090)
![2024-03-09 (4)](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/48303713-2388-4c54-907d-2ffb4fc8e007)
![Captura de tela 2024-03-09 150624](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/a2484b5a-e259-410a-bc35-0ddfbc6f83cd)
![2024-03-09 (5)](https://github.com/elDimasX/Nottext-Data-Protector/assets/51800283/8c1c30b1-2b82-44dd-a34c-e6294eeded7c)

## 1.0.0.2

- Proteção por senha (agora, o aplicativo pode ser bloqueado de ser acessado)
- Compactado (o aplicativo tem somente um arquivo, tornando totalmente portável)
- Proteção adicionadas: Ocultar, Proteger Processo, Não Executar
- Botão de procurar arquivos ou pastas
- Alguns bugs corrigidos e aprimorados
- O driver agora inicia no boot, permitindo a proteção no modo segurança do Windows
- Autoproteção para o driver, agora, ele está mais difícil de ser removido por qualquer usuário
- Multiplas proteções para o mesmo objeto (agora você pode selecionar mais de uma proteção por arquivo ou pasta)

## 1.0.0.1

- Impede modificações por IRP (uma técnica muito avançada para modificar arquivos no kernel)
- MessageBox nos processos que infrijem as regras de arquivos protegidos
- Processos agora podem ser finalizado imedatamente após violar alguma regra de proteção
- Correção de bugs e desempenho corrigido

## 1.0.0.0

- Configurações básicas, como instalar drivers, habilita ou não a proteção global, e etc (básico)
- Ignorar processos, para que eles possam mexer nos objetos protegidos
- Proteje os arquivos contra modificações de somente-leitura ou qualquer acesso

## Avisos
Cuidado com este programa, ele não possui limitações ou restrições de pastas, você pode bloquear tudo, até mesmo o C:\, então escolha suas pastas com cuidado!
