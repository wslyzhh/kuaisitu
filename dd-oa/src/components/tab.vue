<!-- tab切换 -->
<template>
    <div>
        <ul class="tab_list flex flex_s_a" :class="tabClass">
            <li :class="{'active':activeIndex == index}" v-for="(item,index) in tabList" :key="index" @click="changeTab(index)">{{item}}</li>
        </ul>
    </div>
</template>

<script>
export default {
    name:"",
    data() {
       return {
           activeIndex:this.tabIndex
       };
    },
    props:{
        tabList:{
            type:Array,
        },
        tabIndex:{
            type:Number,
            default:0
        },
        tabClass:{
            type:String,
            default:''
        }
    },
    components: {},
    computed: {},
    mounted() {
        
    },
    methods: {
        changeTab(index){
            this.activeIndex = index
            this.$emit('on-tab',index)
        }
    },
}
</script>

<style scoped lang="scss">
    @import '../assets/css/var.scss';
    .tab_list{
        border-bottom: 1px solid $border_bottom_color;
        &.fixed{
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            background-color:#FFF;
        }
        li{
            height: .75rem;
            line-height: .75rem;
            color:$fontSizeColor;
            font-size: $size_30;
        }
        .active{
            color:$blue_1;
            font-weight: bold;
            border-bottom: 2px solid $blue_1;
        }
    }
</style>
