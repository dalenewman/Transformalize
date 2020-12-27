declare const Vue: VueConstructor;

interface CreateElement {
   (tag?: string | Component<any, any, any, any> | AsyncComponent<any, any, any, any> | (() => Component), children?: VNodeChildren): VNode;
   (tag?: string | Component<any, any, any, any> | AsyncComponent<any, any, any, any> | (() => Component), data?: VNodeData, children?: VNodeChildren): VNode;
}

interface Vue {
   readonly $el: Element;
   readonly $options: ComponentOptions<Vue>;
   readonly $parent: Vue;
   readonly $root: Vue;
   readonly $children: Vue[];
   readonly $refs: { [key: string]: Vue | Element | Vue[] | Element[] };
   readonly $slots: { [key: string]: VNode[] | undefined };
   readonly $scopedSlots: { [key: string]: NormalizedScopedSlot | undefined };
   readonly $isServer: boolean;
   readonly $data: Record<string, any>;
   readonly $props: Record<string, any>;
   readonly $ssrContext: any;
   readonly $vnode: VNode;
   readonly $attrs: Record<string, string>;
   readonly $listeners: Record<string, Function | Function[]>;

   $mount(elementOrSelector?: Element | string, hydrating?: boolean): this;
   $forceUpdate(): void;
   $destroy(): void;
   $set: typeof Vue.set;
   $delete: typeof Vue.delete;
   $watch(
      expOrFn: string,
      callback: (this: this, n: any, o: any) => void,
      options?: WatchOptions
   ): (() => void);
   $watch<T>(
      expOrFn: (this: this) => T,
      callback: (this: this, n: T, o: T) => void,
      options?: WatchOptions
   ): (() => void);
   $on(event: string | string[], callback: Function): this;
   $once(event: string | string[], callback: Function): this;
   $off(event?: string | string[], callback?: Function): this;
   $emit(event: string, ...args: any[]): this;
   $nextTick(callback: (this: this) => void): void;
   $nextTick(): Promise<void>;
   $createElement: CreateElement;
}

type CombinedVueInstance<Instance extends Vue, Data, Methods, Computed, Props> = Data & Methods & Computed & Props & Instance;
type ExtendedVue<Instance extends Vue, Data, Methods, Computed, Props> = VueConstructor<CombinedVueInstance<Instance, Data, Methods, Computed, Props> & Vue>;

interface VueConfiguration {
   silent: boolean;
   optionMergeStrategies: any;
   devtools: boolean;
   productionTip: boolean;
   performance: boolean;
   errorHandler(err: Error, vm: Vue, info: string): void;
   warnHandler(msg: string, vm: Vue, trace: string): void;
   ignoredElements: (string | RegExp)[];
   keyCodes: { [key: string]: number | number[] };
   async: boolean;
}

interface VueConstructor<V extends Vue = Vue> {
   new <Data = object, Methods = object, Computed = object, PropNames extends string = never>(options?: ThisTypedComponentOptionsWithArrayProps<V, Data, Methods, Computed, PropNames>): CombinedVueInstance<V, Data, Methods, Computed, Record<PropNames, any>>;
   // ideally, the return type should just contain Props, not Record<keyof Props, any>. But TS requires to have Base constructors with the same return type.
   new <Data = object, Methods = object, Computed = object, Props = object>(options?: ThisTypedComponentOptionsWithRecordProps<V, Data, Methods, Computed, Props>): CombinedVueInstance<V, Data, Methods, Computed, Record<keyof Props, any>>;
   new(options?: ComponentOptions<V>): CombinedVueInstance<V, object, object, object, Record<keyof object, any>>;

   extend<Data, Methods, Computed, PropNames extends string = never>(options?: ThisTypedComponentOptionsWithArrayProps<V, Data, Methods, Computed, PropNames>): ExtendedVue<V, Data, Methods, Computed, Record<PropNames, any>>;
   extend<Data, Methods, Computed, Props>(options?: ThisTypedComponentOptionsWithRecordProps<V, Data, Methods, Computed, Props>): ExtendedVue<V, Data, Methods, Computed, Props>;
   extend<PropNames extends string = never>(definition: FunctionalComponentOptions<Record<PropNames, any>, PropNames[]>): ExtendedVue<V, {}, {}, {}, Record<PropNames, any>>;
   extend<Props>(definition: FunctionalComponentOptions<Props, RecordPropsDefinition<Props>>): ExtendedVue<V, {}, {}, {}, Props>;
   extend(options?: ComponentOptions<V>): ExtendedVue<V, {}, {}, {}, {}>;

   nextTick<T>(callback: (this: T) => void, context?: T): void;
   nextTick(): Promise<void>
   set<T>(object: object, key: string | number, value: T): T;
   set<T>(array: T[], key: number, value: T): T;
   delete(object: object, key: string | number): void;
   delete<T>(array: T[], key: number): void;

   directive(
      id: string,
      definition?: DirectiveOptions | DirectiveFunction
   ): DirectiveOptions;
   filter(id: string, definition?: Function): Function;

   component(id: string): VueConstructor;
   component<VC extends VueConstructor>(id: string, constructor: VC): VC;
   component<Data, Methods, Computed, Props>(id: string, definition: AsyncComponent<Data, Methods, Computed, Props>): ExtendedVue<V, Data, Methods, Computed, Props>;
   component<Data, Methods, Computed, PropNames extends string = never>(id: string, definition?: ThisTypedComponentOptionsWithArrayProps<V, Data, Methods, Computed, PropNames>): ExtendedVue<V, Data, Methods, Computed, Record<PropNames, any>>;
   component<Data, Methods, Computed, Props>(id: string, definition?: ThisTypedComponentOptionsWithRecordProps<V, Data, Methods, Computed, Props>): ExtendedVue<V, Data, Methods, Computed, Props>;
   component<PropNames extends string>(id: string, definition: FunctionalComponentOptions<Record<PropNames, any>, PropNames[]>): ExtendedVue<V, {}, {}, {}, Record<PropNames, any>>;
   component<Props>(id: string, definition: FunctionalComponentOptions<Props, RecordPropsDefinition<Props>>): ExtendedVue<V, {}, {}, {}, Props>;
   component(id: string, definition?: ComponentOptions<V>): ExtendedVue<V, {}, {}, {}, {}>;

   use<T>(plugin: PluginObject<T> | PluginFunction<T>, options?: T): VueConstructor<V>;
   use(plugin: PluginObject<any> | PluginFunction<any>, ...options: any[]): VueConstructor<V>;
   mixin(mixin: VueConstructor | ComponentOptions<Vue>): VueConstructor<V>;
   compile(template: string): {
      render(createElement: typeof Vue.prototype.$createElement): VNode;
      staticRenderFns: (() => VNode)[];
   };

   observable<T>(obj: T): T;

   config: VueConfiguration;
   version: string;
}

type Constructor = {
   new(...args: any[]): any;
}

// we don't support infer props in async component
// N.B. ComponentOptions<V> is contravariant, the default generic should be bottom type
type Component<Data = DefaultData<never>, Methods = DefaultMethods<never>, Computed = DefaultComputed, Props = DefaultProps> =
   | typeof Vue
   | FunctionalComponentOptions<Props>
   | ComponentOptions<never, Data, Methods, Computed, Props>

interface EsModuleComponent {
   default: Component
}

type AsyncComponent<Data = DefaultData<never>, Methods = DefaultMethods<never>, Computed = DefaultComputed, Props = DefaultProps>
   = AsyncComponentPromise<Data, Methods, Computed, Props>
   | AsyncComponentFactory<Data, Methods, Computed, Props>

type AsyncComponentPromise<Data = DefaultData<never>, Methods = DefaultMethods<never>, Computed = DefaultComputed, Props = DefaultProps> = (
   resolve: (component: Component<Data, Methods, Computed, Props>) => void,
   reject: (reason?: any) => void
) => Promise<Component | EsModuleComponent> | void;

type AsyncComponentFactory<Data = DefaultData<never>, Methods = DefaultMethods<never>, Computed = DefaultComputed, Props = DefaultProps> = () => {
   component: AsyncComponentPromise<Data, Methods, Computed, Props>;
   loading?: Component | EsModuleComponent;
   error?: Component | EsModuleComponent;
   delay?: number;
   timeout?: number;
}

/**
 * When the `Computed` type parameter on `ComponentOptions` is inferred,
 * it should have a property with the return type of every get-accessor.
 * Since there isn't a way to query for the return type of a function, we allow TypeScript
 * to infer from the shape of `Accessors<Computed>` and work backwards.
 */
type Accessors<T> = {
   [K in keyof T]: (() => T[K]) | ComputedOptions<T[K]>
}

type DataDef<Data, Props, V> = Data | ((this: Readonly<Props> & V) => Data)
/**
 * This type should be used when an array of strings is used for a component's `props` value.
 */
type ThisTypedComponentOptionsWithArrayProps<V extends Vue, Data, Methods, Computed, PropNames extends string> =
   object &
   ComponentOptions<V, DataDef<Data, Record<PropNames, any>, V>, Methods, Computed, PropNames[], Record<PropNames, any>> &
   ThisType<CombinedVueInstance<V, Data, Methods, Computed, Readonly<Record<PropNames, any>>>>;

/**
 * This type should be used when an object mapped to `PropOptions` is used for a component's `props` value.
 */
type ThisTypedComponentOptionsWithRecordProps<V extends Vue, Data, Methods, Computed, Props> =
   object &
   ComponentOptions<V, DataDef<Data, Props, V>, Methods, Computed, RecordPropsDefinition<Props>, Props> &
   ThisType<CombinedVueInstance<V, Data, Methods, Computed, Readonly<Props>>>;

type DefaultData<V> = object | ((this: V) => object);
type DefaultProps = Record<string, any>;
type DefaultMethods<V> = { [key: string]: (this: V, ...args: any[]) => any };
type DefaultComputed = { [key: string]: any };
interface ComponentOptions<
   V extends Vue,
   Data = DefaultData<V>,
   Methods = DefaultMethods<V>,
   Computed = DefaultComputed,
   PropsDef = PropsDefinition<DefaultProps>,
   Props = DefaultProps> {
   data?: Data;
   props?: PropsDef;
   propsData?: object;
   computed?: Accessors<Computed>;
   methods?: Methods;
   watch?: Record<string, WatchOptionsWithHandler<any> | WatchHandler<any> | string>;

   el?: Element | string;
   template?: string;
   // hack is for functional component type inference, should not be used in user code
   render?(createElement: CreateElement, hack: RenderContext<Props>): VNode;
   renderError?(createElement: CreateElement, err: Error): VNode;
   staticRenderFns?: ((createElement: CreateElement) => VNode)[];

   beforeCreate?(this: V): void;
   created?(): void;
   beforeDestroy?(): void;
   destroyed?(): void;
   beforeMount?(): void;
   mounted?(): void;
   beforeUpdate?(): void;
   updated?(): void;
   activated?(): void;
   deactivated?(): void;
   errorCaptured?(err: Error, vm: Vue, info: string): boolean | void;
   serverPrefetch?(this: V): Promise<void>;

   directives?: { [key: string]: DirectiveFunction | DirectiveOptions };
   components?: { [key: string]: Component<any, any, any, any> | AsyncComponent<any, any, any, any> };
   transitions?: { [key: string]: object };
   filters?: { [key: string]: Function };

   provide?: object | (() => object);
   inject?: InjectOptions;

   model?: {
      prop?: string;
      event?: string;
   };

   parent?: Vue;
   mixins?: (ComponentOptions<Vue> | typeof Vue)[];
   name?: string;
   // TODO: support properly inferred 'extends'
   extends?: ComponentOptions<Vue> | typeof Vue;
   delimiters?: [string, string];
   comments?: boolean;
   inheritAttrs?: boolean;
}

interface FunctionalComponentOptions<Props = DefaultProps, PropDefs = PropsDefinition<Props>> {
   name?: string;
   props?: PropDefs;
   model?: {
      prop?: string;
      event?: string;
   };
   inject?: InjectOptions;
   functional: boolean;
   render?(this: undefined, createElement: CreateElement, context: RenderContext<Props>): VNode | VNode[];
}

interface RenderContext<Props = DefaultProps> {
   props: Props;
   children: VNode[];
   slots(): any;
   data: VNodeData;
   parent: Vue;
   listeners: { [key: string]: Function | Function[] };
   scopedSlots: { [key: string]: NormalizedScopedSlot };
   injections: any
}

type Prop<T> = { (): T } | { new(...args: any[]): T & object } | { new(...args: string[]): Function }

type PropType<T> = Prop<T> | Prop<T>[];

type PropValidator<T> = PropOptions<T> | PropType<T>;

interface PropOptions<T = any> {
   type?: PropType<T>;
   required?: boolean;
   default?: T | null | undefined | (() => T | null | undefined);
   validator?(value: T): boolean;
}

type RecordPropsDefinition<T> = {
   [K in keyof T]: PropValidator<T[K]>
}
type ArrayPropsDefinition<T> = (keyof T)[];
type PropsDefinition<T> = ArrayPropsDefinition<T> | RecordPropsDefinition<T>;

interface ComputedOptions<T> {
   get?(): T;
   set?(value: T): void;
   cache?: boolean;
}

type WatchHandler<T> = (val: T, oldVal: T) => void;

interface WatchOptions {
   deep?: boolean;
   immediate?: boolean;
}

interface WatchOptionsWithHandler<T> extends WatchOptions {
   handler: WatchHandler<T>;
}

interface DirectiveBinding extends Readonly<VNodeDirective> {
   readonly modifiers: { [key: string]: boolean };
}

type DirectiveFunction = (
   el: HTMLElement,
   binding: DirectiveBinding,
   vnode: VNode,
   oldVnode: VNode
) => void;

interface DirectiveOptions {
   bind?: DirectiveFunction;
   inserted?: DirectiveFunction;
   update?: DirectiveFunction;
   componentUpdated?: DirectiveFunction;
   unbind?: DirectiveFunction;
}

type InjectKey = string | symbol;

type InjectOptions = {
   [key: string]: InjectKey | { from?: InjectKey, default?: any }
} | string[];

type PluginFunction<T> = (Vue: Vue, options?: T) => void;

interface PluginObject<T> {
   install: PluginFunction<T>;
   [key: string]: any;
}

type ScopedSlot = (props: any) => ScopedSlotReturnValue;
type ScopedSlotReturnValue = VNode | string | boolean | null | undefined | ScopedSlotReturnArray;
interface ScopedSlotReturnArray extends Array<ScopedSlotReturnValue> { }

// Scoped slots are guaranteed to return Array of VNodes starting in 2.6
type NormalizedScopedSlot = (props: any) => ScopedSlotChildren;
type ScopedSlotChildren = VNode[] | undefined;

// Relaxed type compatible with $createElement
type VNodeChildren = VNodeChildrenArrayContents | [ScopedSlot] | string | boolean | null | undefined;
interface VNodeChildrenArrayContents extends Array<VNodeChildren | VNode> { }

interface VNode {
   tag?: string;
   data?: VNodeData;
   children?: VNode[];
   text?: string;
   elm?: Node;
   ns?: string;
   context?: Vue;
   key?: string | number;
   componentOptions?: VNodeComponentOptions;
   componentInstance?: Vue;
   parent?: VNode;
   raw?: boolean;
   isStatic?: boolean;
   isRootInsert: boolean;
   isComment: boolean;
}

interface VNodeComponentOptions {
   Ctor: typeof Vue;
   propsData?: object;
   listeners?: object;
   children?: VNode[];
   tag?: string;
}

interface VNodeData {
   key?: string | number;
   slot?: string;
   scopedSlots?: { [key: string]: ScopedSlot | undefined };
   ref?: string;
   refInFor?: boolean;
   tag?: string;
   staticClass?: string;
   class?: any;
   staticStyle?: { [key: string]: any };
   style?: string | object[] | object;
   props?: { [key: string]: any };
   attrs?: { [key: string]: any };
   domProps?: { [key: string]: any };
   hook?: { [key: string]: Function };
   on?: { [key: string]: Function | Function[] };
   nativeOn?: { [key: string]: Function | Function[] };
   transition?: object;
   show?: boolean;
   inlineTemplate?: {
      render: Function;
      staticRenderFns: Function[];
   };
   directives?: VNodeDirective[];
   keepAlive?: boolean;
}

interface VNodeDirective {
   name: string;
   value?: any;
   oldValue?: any;
   expression?: any;
   arg?: string;
   oldArg?: string;
   modifiers?: { [key: string]: boolean };
}



