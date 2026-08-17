/** Diagrams for Module 2 - object-oriented programming. */
import { Arrow, Box, C, Defs, Figure, Txt } from './primitives';

/** 2.4 - encapsulation: private state behind a guarded gate. */
export function Encapsulation() {
  const id = 'enc';
  return (
    <Figure
      title="Encapsulation: private fields reachable only through public methods"
      caption="Outside code cannot touch _balance directly - every change passes through a method that enforces the rules."
      viewBox="0 0 640 240"
    >
      <Defs id={id} />

      {/* the capsule */}
      <rect x={230} y={20} width={380} height={200} rx={14} fill={C.surface} stroke={C.border} strokeWidth={1.5} />
      <Txt x={420} y={40} bold color={C.text}>BankAccount</Txt>

      {/* private core */}
      <rect x={420} y={65} width={170} height={130} rx={10} fill={C.wrongSoft} stroke={C.wrong} strokeDasharray="5 4" />
      <Txt x={505} y={85} size={11} color={C.wrong} bold>private</Txt>
      <Box x={440} y={100} w={130} h={34} label="_balance" sub="decimal" mono fill={C.surface} />
      <Txt x={505} y={165} size={10.5}>invisible from outside</Txt>

      {/* public gate */}
      <Box x={255} y={70} w={140} h={34} label="Deposit(amount)" mono fill={C.correctSoft} stroke={C.correct} />
      <Box x={255} y={118} w={140} h={34} label="Withdraw(amount)" mono fill={C.correctSoft} stroke={C.correct} />
      <Box x={255} y={166} w={140} h={34} label="Balance  { get; }" mono fill={C.correctSoft} stroke={C.correct} />

      <Arrow id={id} x1={395} y1={87} x2={435} y2={110} />
      <Arrow id={id} x1={395} y1={135} x2={435} y2={125} />
      <Arrow id={id} x1={435} y1={140} x2={400} y2={175} />

      {/* callers */}
      <Box x={30} y={95} w={150} h={44} label="outside code" sub="account.Deposit(50)" />
      <Arrow id={id} x1={180} y1={117} x2={248} y2={117} color={C.accent} width={2} />

      <Txt x={110} y={170} size={11} color={C.wrong}>account._balance = -5000</Txt>
      <line x1={40} y1={185} x2={180} y2={185} stroke={C.wrong} strokeWidth={1.5} />
      <Txt x={110} y={200} size={10.5} color={C.wrong}>does not compile</Txt>
    </Figure>
  );
}

/** 2.5 - inheritance: a base class and two children. */
export function InheritanceTree() {
  const id = 'inh';
  return (
    <Figure
      title="Inheritance: Dog and Cat both derive from Animal"
      caption="The hollow triangle points at the base class. Children inherit Name, Age and Describe(), and add their own members."
      viewBox="0 0 640 250"
    >
      <Defs id={id} />

      <Box x={240} y={16} w={160} h={64} label="Animal" sub="Name · Age · Describe()" bold />

      <Box x={80} y={160} w={180} h={64} label="Dog" sub="+ Breed · Fetch()" />
      <Box x={380} y={160} w={180} h={64} label="Cat" sub="+ Purr()" />

      {/* hollow triangles point UP at the base: "is a" */}
      <Arrow id={id} x1={170} y1={158} x2={288} y2={84} head="triangle" />
      <Arrow id={id} x1={470} y1={158} x2={352} y2={84} head="triangle" />

      <Txt x={185} y={110} size={11}>is an</Txt>
      <Txt x={455} y={110} size={11}>is an</Txt>

      <Txt x={320} y={240} size={11}>
        rex.Describe() works even though Dog never defines it - it was inherited
      </Txt>
    </Figure>
  );
}

/** 2.6 - polymorphism: one call site, three method bodies. */
export function PolymorphismDispatch() {
  const id = 'poly';
  return (
    <Figure
      title="Polymorphism: shape.Area() runs a different body per real type"
      caption="Every element is declared Shape, but the runtime looks at the REAL object and dispatches to its own Area()."
      viewBox="0 0 640 260"
    >
      <Defs id={id} />

      <Box x={20} y={95} w={190} h={70} label="foreach (Shape s ...)" sub="s.Area()" mono bold
        fill={C.soft} stroke={C.accent} />

      <Box x={330} y={20} w={200} h={54} label="Circle" sub="Math.PI * r * r" mono />
      <Box x={330} y={103} w={200} h={54} label="Rectangle" sub="width * height" mono />
      <Box x={330} y={186} w={200} h={54} label="Triangle" sub="Heron's formula" mono />

      <Arrow id={id} x1={212} y1={112} x2={324} y2={50} color={C.accent} width={1.8} />
      <Arrow id={id} x1={212} y1={130} x2={324} y2={130} color={C.accent} width={1.8} />
      <Arrow id={id} x1={212} y1={148} x2={324} y2={210} color={C.accent} width={1.8} />

      <Txt x={268} y={68} size={10.5}>if it IS a Circle</Txt>
      <Txt x={268} y={118} size={10.5}>if a Rectangle</Txt>
      <Txt x={268} y={192} size={10.5}>if a Triangle</Txt>

      <Txt x={575} y={47}  mono size={11} color={C.correct}>28.27</Txt>
      <Txt x={575} y={130} mono size={11} color={C.correct}>20</Txt>
      <Txt x={575} y={213} mono size={11} color={C.correct}>6</Txt>
    </Figure>
  );
}

/** 2.7 - interfaces: one contract, three unrelated implementations. */
export function InterfaceContract() {
  const id = 'ifc';
  return (
    <Figure
      title="One IStorage interface implemented by three unrelated classes"
      caption="Dashed arrows are UML 'realises'. SaveGame depends only on the contract, so any implementation can be swapped in."
      viewBox="0 0 640 250"
    >
      <Defs id={id} />

      <Box x={240} y={16} w={160} h={58} label="«interface»" sub="IStorage: Save · Load" bold
        fill={C.soft} stroke={C.accent} />

      <Box x={40} y={170} w={160} h={48} label="MemoryStorage" sub="a Dictionary" />
      <Box x={240} y={170} w={160} h={48} label="FileStorage" sub="the disk" />
      <Box x={440} y={170} w={160} h={48} label="CloudStorage" sub="a web API" />

      <Arrow id={id} x1={120} y1={168} x2={280} y2={78} head="triangle" dashed />
      <Arrow id={id} x1={320} y1={168} x2={320} y2={78} head="triangle" dashed />
      <Arrow id={id} x1={520} y1={168} x2={360} y2={78} head="triangle" dashed />

      <Txt x={320} y={240} size={11}>
        SaveGame(IStorage storage) - handed any of the three, it neither knows nor cares which
      </Txt>
    </Figure>
  );
}

/** 2.8 - value vs reference assignment. */
export function ValueVsReference() {
  const id = 'vvr';
  return (
    <Figure
      title="Assigning a struct copies the value; assigning a class copies the reference"
      caption="b is an independent copy, so changing it leaves a alone. c and d are two arrows to ONE heap object, so a change through d shows through c."
      viewBox="0 0 640 300"
    >
      <Defs id={id} />

      {/* left: value types */}
      <Txt x={160} y={20} bold color={C.text}>struct  (value type)</Txt>
      <Txt x={160} y={40} mono size={11}>var b = a;</Txt>

      <Box x={55} y={60} w={95} h={52} label="a" sub="X=1, Y=2" mono />
      <Box x={175} y={60} w={95} h={52} label="b" sub="X=99, Y=2" mono />
      <Arrow id={id} x1={152} y1={86} x2={172} y2={86} color={C.correct} />
      <Txt x={162} y={135} size={10.5} color={C.correct}>a full, independent copy</Txt>
      <Txt x={162} y={155} size={10.5}>b.X = 99 changed only b</Txt>

      {/* divider */}
      <line x1={320} y1={15} x2={320} y2={285} stroke={C.border} />

      {/* right: reference types */}
      <Txt x={480} y={20} bold color={C.text}>class  (reference type)</Txt>
      <Txt x={480} y={40} mono size={11}>var d = c;</Txt>

      <Box x={360} y={60} w={70} h={40} label="c" mono />
      <Box x={360} y={130} w={70} h={40} label="d" mono />

      {/* the heap object */}
      <rect x={490} y={75} width={120} height={80} rx={10} fill={C.soft} stroke={C.accent} strokeWidth={1.5} />
      <Txt x={550} y={95} size={11} color={C.accent} bold>heap object</Txt>
      <Txt x={550} y={120} mono size={12} color={C.text}>X=99, Y=2</Txt>

      <Arrow id={id} x1={432} y1={80} x2={485} y2={100} color={C.accent} width={1.8} />
      <Arrow id={id} x1={432} y1={150} x2={485} y2={130} color={C.accent} width={1.8} />

      <Txt x={487} y={185} size={10.5} color={C.wrong}>two arrows, ONE object</Txt>
      <Txt x={487} y={205} size={10.5}>d.X = 99 ... and c.X is 99 too</Txt>

      <Txt x={162} y={205} size={10.5} mono>a.X == 1   still</Txt>

      <Txt x={320} y={265} size={11}>
        The single most bug-explaining distinction in C#
      </Txt>
    </Figure>
  );
}

/** 2.12 - the five UML relationship arrows. */
export function UmlRelationships() {
  const id = 'uml';
  const rows: { y: number; from: string; to: string; label: string; head: 'triangle' | 'arrow' | 'none'; dashed: boolean; diamond?: 'filled' | 'hollow' }[] = [
    { y: 40,  from: 'Dog',     to: 'Animal',   label: 'inheritance - IS-A',            head: 'triangle', dashed: false },
    { y: 90,  from: 'Duck',    to: 'IFlyer',   label: 'realisation - implements',      head: 'triangle', dashed: true },
    { y: 140, from: 'House',   to: 'Room',     label: 'composition - owns; part dies with whole', head: 'none', dashed: false, diamond: 'filled' },
    { y: 190, from: 'Team',    to: 'Player',   label: 'aggregation - has; part lives on',        head: 'none', dashed: false, diamond: 'hollow' },
    { y: 240, from: 'Printer', to: 'Document', label: 'dependency - uses briefly',     head: 'arrow', dashed: true },
  ];
  return (
    <Figure
      title="The five UML class relationships and their arrows"
      caption="The diamond sits on the OWNING end: filled means the part cannot outlive the whole, hollow means it can."
      viewBox="0 0 640 280"
    >
      <Defs id={id} />
      {rows.map((r) => (
        <g key={r.y}>
          <Box x={20} y={r.y - 17} w={110} h={34} label={r.from} mono />
          <Box x={250} y={r.y - 17} w={110} h={34} label={r.to} mono />
          <Arrow id={id} x1={r.diamond ? 152 : 133} y1={r.y} x2={246} y2={r.y}
            head={r.head} dashed={r.dashed} />
          {r.diamond && (
            <path d={`M 134 ${r.y} l 10 -6 l 10 6 l -10 6 z`}
              fill={r.diamond === 'filled' ? C.muted : C.surface}
              stroke={C.muted} strokeWidth={1.3} />
          )}
          <Txt x={385} y={r.y} anchor="start" size={11.5}>{r.label}</Txt>
        </g>
      ))}
    </Figure>
  );
}
