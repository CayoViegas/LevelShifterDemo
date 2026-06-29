# LevelShifter Demonstration

## 1. Atores e Componentes (GameObjects)

- **Player (O Jogador)**
    - **Tag:** `Player`
    - **Componentes:**
        - `Rigidbody2D`: Controla a física. O `Body Type` é *Dynamic*. Em `Constraints`, *Freeze Rotation Z* deve estar marcado. O `Mateiral` recebe um material física de atrito zero (*ZeroFriction*).
        - `BoxCollider2D`: A hitbox física do personagem.
        - `PlayerController (Script)`: Lê o input de movimento, checa o chão via OverlapBox e aplica velocidade vetorial ao Rigidbody.

- **MapBlock (O Prefab do Mapa)**
    - **Função:** Contêiner modular que representa uma sala inteira (tamanho padrão de 20x12 unidades).
    - **Componentes no objeto Pai:**
        - `BoxCollider2D`: Marcado como *Is Trigger*, tamanho `X = 20, Y = 12`. Serve como sensor para saber se o jogador está dentro da sala e para trocar a câmera.
        - `MapBlock (Script)`: Guarda a coordenada da matriz e expõe métodos para ligar/desligar o visual.
        - `MapRoom (Script)`: Lê o Trigger e liga/desliga a câmera local.
    - **Objetos Filhos (Hierarquia Interna):**
        - `Grid` > `Tilemap`: Contém a arte do chão e paredes. Possui um `Tilemap Collider 2D` configurado com a Layer `Ground`.
        - `HoverOverlay (Sprite)`: Um quadrado translúcido de 20x12 que indica o cursor do Level Shifter. (Desativado por padrão).
        - `KanjiIcon (Sprite)`: O ícone de seleção ativa. (Desativado por padrão).
        - `LocalCamera (Cinemachine 2D)`: Fica travada no centro local da sala (`10, 6`), com prioridade padrão `10`.
- **GameManager (O Gerente)**
    - **Função:** O cérebro que gerencia a matriz lógica e paralisa o jogo.
    - **Componentes:**
        - `LevelShifterManager (Script)`: Ouve a tecla *C*, pausa o input do jogador, gerencia o array bidimensional `MapBlock[,]`, aplica travas de estado (`isSwapping`) e move fisicamente as instâncias via Coroutine.
- **Global Camera**
    - **Função:** A câmera que enxerga o tabuleiro todo.
    - **Componentes:**
        - `Cinemachine 2D Camera`: Posição Z configurada para `-10`. *Orthographic Size* expandido (ex: 15). **Priority** configurada para `20` (garante que ela sobreponha as câmeras locais). Inicia desativada.

## 2. Fluxo de Execução do Level Shifter
1. Jogador aperta a tecla de ativação.
2. `LevelShifterManager` liga a Global Camera (que rouba a visão devido à Prioridade 20).
3. O `PlayerController` é desligado e o Rigidbody2D do jogador vira *Kinematic* (congelado no espaço).
4. O Gerente usa `bounds.Contains` para ver dentro de qual colisor de mapa o jogador está, e faz o jogador virar "Filho" (Child) daquele mapa.
5. O jogador navega na matriz virtual usando setas e seleciona com a tecla "X".
6. Ao permutar, as posições dos `MapBlocks` são invertidas. O jogador (como é filho) viaja automaticamente sem interagir com a física.
7. Ao fechar o modo, o jogador perde o "Pai" e a física *Dynamic* é reativada, assumindo seu lugar no novo layout de mundo.

## 3. Detalhes Técnicos: Animação e Interpolação
Para evitar teletransportes abruptos e melhorar o *game feel*, a troca física dos mapas é gerenciada assincronamente por uma **Coroutine** (`AnimateSwap`).

- **Trava de Estado (State Lock):** A variável booleana `isSwapping` bloqueia novos inputs de seleção do jogador enquanto uma animação está em curso, garantindo a integridade da matriz.
- **Movimento (Lerp):** O deslocamento dos mapas no espaço 3D é calculado frame a frame usando `Vector3.Lerp` durante um intervalo de tempo fixo (ex: 0.4s).
- **Curva de Suavização:** A porcentagem de tempo decorrido passa por uma função `Mathf.SmoothStep`, gerando um movimento não-linear (acelera no início e freia suavemente no fim).
- **Correção de Flutuação (Snap Final):** Ao fim da Coroutine, as coordenadas espaciais dos mapas são cravadas exatamente nos valores matemáticos previstos para evitar desalinhamentos por erros de ponto flutuante (*floating-point errors*) a longo prazo.