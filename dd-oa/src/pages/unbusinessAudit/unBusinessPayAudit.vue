<!-- 审批业务支付申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c">
                <label class="title"><span>审批类型</span></label>
                <!-- <input type="text" readonly :value="typeName" placeholder="请选择审批类型（必填）">
                <div class="icon_right arrows_right"></div> -->
                <h3 class="hint_1">{{typeName}}</h3>
            </li>
            <li class="flex flex_a_c" @click="chosen('flag')">
                <label class="title"><span>审批状态</span></label>
                <input type="text" readonly :value="flagName" placeholder="请选择审批状态（必填）">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="addData.remarks" placeholder="请输入备注"></textarea>
            </li>
        </ul>
        <top-nav title="非业务支付审批" text="保存" @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {mapActions} from 'vuex'
export default {
    name:"",
    data() {
        return {
            addData:{},
            clientId:0, 
            typeList:[  //类型
                {
                    key:'部门审批',
                    value:'1'
                },
                {
                    key:'财务审批',
                    value:'2'
                },
                {
                    key:'总经理审批',
                    value:'3'
                },
            ],
            typeId:0,
            typeName:'',
            flag:[  //审核
                {
                    key:'待审',
                    value:'0'
                },
                {
                    key:'未通过',
                    value:'1'
                },
                {
                    key:'通过',
                    value:'2'
                },
            ],
            flagId:0,
            flagName:"",
            then:0,
        };
    },
    components: {},
    computed: {},
    created(){
        let {id} = this.$route.query
        let params = {
            ubaid:id,
            managerid:24
        }
        // 初始化数据
        this.getUnBusinessPayAuditObtain(params).then(res => {
            this.addData = res.data
            if(!this.addData.status){                    
                this.ddSet.setToast({text:res.data.msg}).then(res => {
                    this.$router.go(-1)
                })
            }else{
                this.clientId = id
                this.typeId = this.typeList[this.addData.type-1].value
                this.typeName = this.typeList[this.addData.type-1].key
                this.flagId = this.flag[this.addData.flag].value
                this.flagName = this.flag[this.addData.flag].key                
            }
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getUnBusinessPayAuditObtain',
            'getUnBusinessPayAudit'
        ]),
        submit(item){ //提交
            if(!this.flagName){
                //console.log(this.flagName)
                this.ddSet.setToast({text:'请您选择审批状态'})
                return
            }
            this.addData.uba_id = this.clientId
            this.addData.ctype = this.typeId
            //console.log(this.addData.ctype)
            this.addData.cstatus = this.flagId
            //console.log(this.flagId)
            this.addData.managerid = 24 //测试ID
            this.ddSet.showLoad()
            this.getUnBusinessPayAudit(this.addData).then(res => {
                this.ddSet.hideLoad()
                if(res.data.status){                    
                    this.ddSet.setToast({text:'审批信息成功'}).then(res => {
                        this.$router.go(-1)
                    })
                }else{
                    this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                this.ddSet.hideLoad()
            })           
        },
        chosen(item){
            let selectedKey,source
            if(item == 'flag'){
                source = this.flag
                selectedKey = this.flagName
            }
            this.ddSet.setChosen({source,selectedKey}).then(res => {
                if(item == 'flag'){
                    this.$set(this,'flagName',res.key)
                    this.$set(this,'flagId',res.value)
                }
            })
        },
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>

