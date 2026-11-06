import { useEffect, useRef } from 'react';
import { defaultKeymap, history, historyKeymap, indentWithTab } from '@codemirror/commands';
import { bracketMatching, indentOnInput, indentUnit, StreamLanguage, syntaxHighlighting } from '@codemirror/language';
import { csharp } from '@codemirror/legacy-modes/mode/clike';
import { EditorState } from '@codemirror/state';
import { EditorView, keymap, lineNumbers } from '@codemirror/view';
import { classHighlighter } from '@lezer/highlight';

interface Props {
  value: string;
  onChange: (value: string) => void;
  /** Called for Ctrl/Cmd+Enter. */
  onRun: () => void;
  label: string;
}

/**
 * A small C# editor. It is CodeMirror rather than Monaco because it works on phones and adds far
 * less to the download. Loaded on demand, so only students who open it pay for it. Colours come
 * from index.css, so it follows the site's themes.
 */
export default function CodeEditor({ value, onChange, onRun, label }: Props) {
  const host = useRef<HTMLDivElement>(null);
  const view = useRef<EditorView | null>(null);
  const callbacks = useRef({ onChange, onRun });
  useEffect(() => {
    callbacks.current = { onChange, onRun };
  });

  useEffect(() => {
    const editor = new EditorView({
      parent: host.current!,
      state: EditorState.create({
        doc: value,
        extensions: [
          lineNumbers(),
          history(),
          indentOnInput(),
          bracketMatching(),
          indentUnit.of('    '),
          StreamLanguage.define(csharp),
          syntaxHighlighting(classHighlighter),
          EditorView.contentAttributes.of({ 'aria-label': label, spellcheck: 'false', autocapitalize: 'off', autocorrect: 'off' }),
          keymap.of([
            {
              key: 'Mod-Enter',
              run: () => {
                callbacks.current.onRun();
                return true;
              },
            },
            // Tab indents. Pressing Escape first lets keyboard users Tab out of the editor.
            indentWithTab,
            ...defaultKeymap,
            ...historyKeymap,
          ]),
          EditorView.updateListener.of((update) => {
            if (update.docChanged) callbacks.current.onChange(update.state.doc.toString());
          }),
        ],
      }),
    });
    view.current = editor;
    return () => editor.destroy();
    // Created once. Later changes to `value` are applied by the effect below.
  }, []);

  // Picks up a change from outside, such as "Reset to starter".
  useEffect(() => {
    const editor = view.current;
    if (editor && editor.state.doc.toString() !== value) {
      editor.dispatch({ changes: { from: 0, to: editor.state.doc.length, insert: value } });
    }
  }, [value]);

  return <div ref={host} className="code-editor" />;
}
