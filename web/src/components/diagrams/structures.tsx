/** Diagrams for Modules 4 and 5 - memory, async, and the data structures. */
import { Arrow, Box, C, Defs, Figure, Txt } from './primitives';

/** 4.8 - the stack, the heap and a reference between them. */
export function StackHeap() {
  const id = 'sh';
  return (
    <Figure
      title="Stack and heap: value locals live in the frame, objects live on the heap"
      caption="The frame vanishes the instant the method returns. The heap object stays until the garbage collector proves nothing points at it."
      viewBox="0 0 640 270"
    >
      <Defs id={id} />

      {/* stack */}
      <rect x={40} y={35} width={200} height={205} rx={10} fill="none" stroke={C.border} strokeWidth={1.5} />
      <Txt x={140} y={20} bold color={C.text}>the stack</Txt>

      <rect x={55} y={50} width={170} height={80} rx={8} fill={C.surface} stroke={C.border} />
      <Txt x={140} y={64} size={10.5}>Main's frame</Txt>
      <Box x={68} y={78} w={144} h={20} label="int count = 3" mono fill={C.correctSoft} stroke={C.correct} />
      <Box x={68} y={102} w={144} h={20} label="Player p  ●───" mono fill={C.soft} stroke={C.accent} />

      <rect x={55} y={145} width={170} height={55} rx={8} fill={C.surface} stroke={C.border} />
      <Txt x={140} y={160} size={10.5}>Update's frame</Txt>
      <Box x={68} y={172} w={144} h={20} label="double dt = 0.16" mono fill={C.correctSoft} stroke={C.correct} />

      <Txt x={140} y={222} size={10.5}>freed automatically on return</Txt>

      {/* heap */}
      <rect x={330} y={35} width={280} height={205} rx={10} fill="none" stroke={C.border} strokeWidth={1.5} strokeDasharray="6 5" />
      <Txt x={470} y={20} bold color={C.text}>the heap</Txt>

      <rect x={355} y={60} width={160} height={78} rx={10} fill={C.soft} stroke={C.accent} strokeWidth={1.5} />
      <Txt x={435} y={78} size={11.5} color={C.accent} bold>Player object</Txt>
      <Txt x={435} y={100} mono size={11} color={C.text}>Name = "Ada"</Txt>
      <Txt x={435} y={118} mono size={11} color={C.text}>Health = 100</Txt>

      <Box x={355} y={160} w={110} h={34} label="forgotten obj" fill={C.wrongSoft} stroke={C.wrong} />
      <Txt x={410} y={210} size={10.5} color={C.wrong}>no arrows in - garbage,</Txt>
      <Txt x={410} y={226} size={10.5} color={C.wrong}>collected by the GC</Txt>

      <Arrow id={id} x1={214} y1={112} x2={350} y2={95} color={C.accent} width={1.8} />
      <Txt x={280} y={90} size={10.5}>the reference</Txt>
    </Figure>
  );
}

/** 4.10 - sequential awaits versus Task.WhenAll. */
export function AsyncTimeline() {
  const id = 'at';
  const bar = (x: number, y: number, w: number, label: string) => (
    <g>
      <rect x={x} y={y} width={w} height={22} rx={5} fill={C.soft} stroke={C.accent} />
      <Txt x={x + w / 2} y={y + 11} size={10.5} mono color={C.text}>{label}</Txt>
    </g>
  );
  return (
    <Figure
      title="Awaiting one at a time versus starting all three then awaiting once"
      caption="Sequential awaits still add up to 450ms. Start the tasks first and the waits overlap: 150ms total, same work."
      viewBox="0 0 640 250"
    >
      <Defs id={id} />

      <Txt x={12} y={30} anchor="start" bold color={C.text}>await one at a time</Txt>
      {bar(120, 45, 140, 'A  150ms')}
      {bar(263, 45, 140, 'B  150ms')}
      {bar(406, 45, 140, 'C  150ms')}
      <Txt x={560} y={56} anchor="start" size={11.5} color={C.wrong} bold>450ms</Txt>

      <line x1={120} y1={95} x2={546} y2={95} stroke={C.border} />

      <Txt x={12} y={125} anchor="start" bold color={C.text}>Task.WhenAll</Txt>
      {bar(120, 140, 140, 'A  150ms')}
      {bar(120, 168, 140, 'B  150ms')}
      {bar(120, 196, 140, 'C  150ms')}
      <Txt x={268} y={168} anchor="start" size={11.5} color={C.correct} bold>150ms</Txt>

      <Txt x={330} y={200} anchor="start" size={11}>
        the waits overlap - start first, await after
      </Txt>
    </Figure>
  );
}

/** 5.1 - the growth curves, sketched. */
export function BigOCurves() {
  const id = 'bo';
  return (
    <Figure
      title="How each complexity class grows with input size"
      caption="At small n every algorithm looks fine. The curves separate brutally as n grows - which is the whole point of Big-O."
      viewBox="0 0 640 260"
    >
      <Defs id={id} />

      {/* axes */}
      <Arrow id={id} x1={50} y1={225} x2={600} y2={225} />
      <Arrow id={id} x1={50} y1={225} x2={50} y2={20} />
      <Txt x={590} y={242} size={11}>n (input size)</Txt>
      <Txt x={28} y={30} size={11}>work</Txt>

      {/* curves drawn as paths; anchored at the origin-ish */}
      <path d="M 50 222 L 590 218" fill="none" stroke={C.correct} strokeWidth={2} />
      <Txt x={612} y={218} size={10.5} mono color={C.correct}>O(1)</Txt>

      <path d="M 50 222 Q 150 195 590 185" fill="none" stroke={C.correct} strokeWidth={2} strokeDasharray="6 4" />
      <Txt x={615} y={185} size={10.5} mono color={C.correct}>O(log n)</Txt>

      <path d="M 50 222 L 590 120" fill="none" stroke={C.accent} strokeWidth={2} />
      <Txt x={610} y={118} size={10.5} mono color={C.accent}>O(n)</Txt>

      <path d="M 50 222 Q 300 170 590 60" fill="none" stroke={C.accent} strokeWidth={2} strokeDasharray="6 4" />
      <Txt x={618} y={58} size={10.5} mono color={C.accent}>O(n log n)</Txt>

      <path d="M 50 222 Q 330 215 480 25" fill="none" stroke={C.wrong} strokeWidth={2} />
      <Txt x={500} y={22} size={10.5} mono color={C.wrong}>O(n²)</Txt>

      <path d="M 50 222 Q 240 218 300 25" fill="none" stroke={C.wrong} strokeWidth={2} strokeDasharray="6 4" />
      <Txt x={318} y={22} size={10.5} mono color={C.wrong}>O(2ⁿ)</Txt>
    </Figure>
  );
}

/** 5.5 - a singly linked list. */
export function LinkedListNodes() {
  const id = 'll';
  const node = (x: number, v: string) => (
    <g>
      <rect x={x} y={80} width={80} height={44} rx={8} fill={C.surface} stroke={C.border} strokeWidth={1.4} />
      <line x1={x + 52} y1={80} x2={x + 52} y2={124} stroke={C.border} />
      <Txt x={x + 26} y={102} mono size={13} color={C.text} bold>{v}</Txt>
      <circle cx={x + 66} cy={102} r={3.5} fill={C.accent} />
    </g>
  );
  return (
    <Figure
      title="A singly linked list: each node holds a value and a pointer to the next"
      caption="head and tail are just references. Inserting at the front is two pointer writes - nothing shuffles, unlike an array."
      viewBox="0 0 640 200"
    >
      <Defs id={id} />

      <Txt x={80} y={35} mono size={12} color={C.accent} bold>head</Txt>
      <Arrow id={id} x1={80} y1={48} x2={104} y2={78} color={C.accent} width={1.8} />

      {node(70, '10')}
      {node(240, '20')}
      {node(410, '30')}

      <Arrow id={id} x1={138} y1={102} x2={235} y2={102} color={C.accent} width={1.8} />
      <Arrow id={id} x1={308} y1={102} x2={405} y2={102} color={C.accent} width={1.8} />

      {/* null terminator */}
      <Arrow id={id} x1={478} y1={102} x2={540} y2={102} color={C.accent} width={1.8} head="none" />
      <Txt x={562} y={102} mono size={12}>null</Txt>

      <Txt x={445} y={35} mono size={12} color={C.accent} bold>tail</Txt>
      <Arrow id={id} x1={445} y1={48} x2={442} y2={78} color={C.accent} width={1.8} />

      <Txt x={320} y={165} size={11}>
        walk the arrows to search: O(n) - there is no index arithmetic
      </Txt>
    </Figure>
  );
}

/** 5.6 - the circular queue, wrapped. */
export function CircularQueue() {
  const id = 'cq';
  // exactly the lesson's state: array [Eve, _, Cara, Dev], head=2, tail=1
  const slots = [
    { label: 'Eve', filled: true },
    { label: '', filled: false },
    { label: 'Cara', filled: true },
    { label: 'Dev', filled: true },
  ];
  return (
    <Figure
      title="A circular queue whose contents have wrapped around the array"
      caption="Logically the queue is Cara, Dev, Eve - but Eve physically sits at index 0. head and tail advance with % and nothing ever shuffles."
      viewBox="0 0 640 230"
      maxWidth={560}
    >
      <Defs id={id} />

      {slots.map((s, i) => (
        <g key={i}>
          <rect x={120 + i * 100} y={70} width={90} height={54} rx={8}
            fill={s.filled ? C.soft : C.surface}
            stroke={s.filled ? C.accent : C.border} strokeWidth={1.4} />
          {s.label
            ? <Txt x={165 + i * 100} y={97} mono size={13} color={C.text} bold>{s.label}</Txt>
            : <Txt x={165 + i * 100} y={97} mono size={13}>_</Txt>}
          <Txt x={165 + i * 100} y={140} mono size={10.5}>[{i}]</Txt>
        </g>
      ))}

      <Txt x={365} y={30} mono size={12} color={C.correct} bold>head = 2</Txt>
      <Arrow id={id} x1={365} y1={42} x2={365} y2={66} color={C.correct} width={1.8} />

      <Txt x={230} y={190} mono size={12} color={C.wrong} bold>tail = 1</Txt>
      <Arrow id={id} x1={230} y1={178} x2={230} y2={128} color={C.wrong} width={1.8} />

      {/* the wrap: an arc from the end of slot 3 back to slot 0 */}
      <path d="M 520 97 C 590 97 590 40 520 40 L 130 40 C 95 40 95 66 118 68"
        fill="none" stroke={C.muted} strokeWidth={1.4} strokeDasharray="5 4"
        markerEnd={`url(#${id}-arrow)`} />
      <Txt x={320} y={54} size={10.5}>(tail + 1) % 4 wraps past the end back to 0</Txt>
    </Figure>
  );
}

/** 5.7 - the lesson's binary search tree. */
export function BinarySearchTree() {
  const id = 'bst';
  const n = (x: number, y: number, v: string, hot = false) => (
    <g>
      <circle cx={x} cy={y} r={20} fill={hot ? C.soft : C.surface}
        stroke={hot ? C.accent : C.border} strokeWidth={1.6} />
      <Txt x={x} y={y} mono size={12.5} color={C.text} bold>{v}</Txt>
    </g>
  );
  const edge = (x1: number, y1: number, x2: number, y2: number) => (
    <line x1={x1} y1={y1} x2={x2} y2={y2} stroke={C.border} strokeWidth={1.6} />
  );
  return (
    <Figure
      title="The binary search tree built by inserting 50, 30, 70, 20, 40, 60, 80, 35"
      caption="Smaller to the left, larger to the right, at every node. Searching for 35 follows the highlighted path: 50 -> 30 -> 40 -> 35, four comparisons."
      viewBox="0 0 640 260"
    >
      <Defs id={id} />

      {edge(320, 45, 180, 105)}
      {edge(320, 45, 460, 105)}
      {edge(180, 105, 110, 170)}
      {edge(180, 105, 250, 170)}
      {edge(460, 105, 390, 170)}
      {edge(460, 105, 530, 170)}
      {edge(250, 170, 215, 228)}

      {n(320, 45, '50', true)}
      {n(180, 105, '30', true)}
      {n(460, 105, '70')}
      {n(110, 170, '20')}
      {n(250, 170, '40', true)}
      {n(390, 170, '60')}
      {n(530, 170, '80')}
      {n(215, 228, '35', true)}

      <Txt x={545} y={45} anchor="start" size={10.5}>height 4</Txt>
      <Txt x={545} y={62} anchor="start" size={10.5}>search = O(height)</Txt>
    </Figure>
  );
}

/** 5.8 - the lesson's graph, as nodes and edges. */
export function GraphMap() {
  const id = 'gm';
  const pos: Record<string, [number, number]> = {
    A: [90, 60], B: [250, 60], C: [90, 180], D: [250, 180], E: [420, 180], F: [420, 60],
  };
  const edges: [string, string][] = [['A','B'],['A','C'],['B','C'],['B','D'],['C','D'],['D','E'],['E','F']];
  return (
    <Figure
      title="An undirected graph of six nodes"
      caption="No shape rules at all - cycles allowed, any node may link to any other. BFS from A reaches F in four hops: A, B, D, E, F."
      viewBox="0 0 640 250"
      maxWidth={520}
    >
      <Defs id={id} />
      {edges.map(([a, b]) => (
        <line key={a + b} x1={pos[a][0]} y1={pos[a][1]} x2={pos[b][0]} y2={pos[b][1]}
          stroke={C.border} strokeWidth={1.8} />
      ))}
      {/* shortest path A-B-D-E-F highlighted */}
      {([['A','B'],['B','D'],['D','E'],['E','F']] as [string, string][]).map(([a, b]) => (
        <line key={'p' + a + b} x1={pos[a][0]} y1={pos[a][1]} x2={pos[b][0]} y2={pos[b][1]}
          stroke={C.accent} strokeWidth={2.6} />
      ))}
      {Object.entries(pos).map(([name, [x, y]]) => (
        <g key={name}>
          <circle cx={x} cy={y} r={21}
            fill={'ABDEF'.includes(name) ? C.soft : C.surface}
            stroke={'ABDEF'.includes(name) ? C.accent : C.border} strokeWidth={1.6} />
          <Txt x={x} y={y} mono size={13} color={C.text} bold>{name}</Txt>
        </g>
      ))}
      <Txt x={255} y={235} size={11}>highlighted: the shortest A to F, found by breadth-first search</Txt>
    </Figure>
  );
}
