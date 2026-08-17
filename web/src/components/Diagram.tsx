import type { ComponentType } from 'react';
import {
  Encapsulation, InheritanceTree, InterfaceContract, PolymorphismDispatch,
  UmlRelationships, ValueVsReference,
} from './diagrams/oop';
import {
  AsyncTimeline, BigOCurves, BinarySearchTree, CircularQueue, GraphMap,
  LinkedListNodes, StackHeap,
} from './diagrams/structures';

/**
 * Every diagram the notes can reference, by name. A note embeds one with a fence:
 *
 *     ```diagram
 *     inheritance-tree
 *     ```
 *
 * On GitHub the fence degrades to a code block naming the diagram; on the site it
 * renders as themed, inline SVG.
 */
const REGISTRY: Record<string, ComponentType> = {
  encapsulation: Encapsulation,
  'inheritance-tree': InheritanceTree,
  'polymorphism-dispatch': PolymorphismDispatch,
  'interface-contract': InterfaceContract,
  'value-vs-reference': ValueVsReference,
  'uml-relationships': UmlRelationships,
  'stack-heap': StackHeap,
  'async-timeline': AsyncTimeline,
  'big-o-curves': BigOCurves,
  'linked-list': LinkedListNodes,
  'circular-queue': CircularQueue,
  bst: BinarySearchTree,
  'graph-map': GraphMap,
};

export function Diagram({ name }: { name: string }) {
  const Found = REGISTRY[name];

  if (!Found) {
    return <p className="missing">Unknown diagram “{name}”.</p>;
  }

  return <Found />;
}
