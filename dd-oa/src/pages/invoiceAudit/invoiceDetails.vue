<!-- 查看发票详细信息 -->
<template>
    <div class="body_gery" v-if="isDatails">
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>订单号</span></label>
                 {{datails.inv_oid}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>客户</span></label>
                {{datails.c_name}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>购买方名称</span></label>
                {{datails.inv_purchaserName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>纳税人编号</span></label>
                {{datails.inv_purchaserNum}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>地址</span></label>
                {{datails.inv_purchaserAddress}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>电话</span></label>
                {{datails.inv_purchaserPhone}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开户行</span></label>
                {{datails.inv_purchaserBank}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>银行账号</span></label>
                {{datails.inv_purchaserBankNum}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>应税劳务、服务名称</span></label>
                {{datails.inv_serviceType}} - {{datails.inv_serviceName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>专普票</span></label>
                {{datails.inv_type==null?'':datails.inv_type==true?'专票':'普票'}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开票金额</span></label>
                {{datails.inv_money}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>申请时超开</span></label>
                {{datails.inv_overMoney}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>送票方式</span></label>
                {{datails.inv_sentWay}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开票区域</span></label>
                {{datails.de_subname}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人名称</span></label>
                {{datails.inv_receiveName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人电话</span></label>
                {{datails.inv_receivePhone}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人地址</span></label>
                {{datails.inv_receiveAddress}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>备注</span></label>
                {{datails.inv_remark}}
            </li>
            <li class="flex flex_a_c flex_s_b">
			    <label class="title newTitle"><span>审批类型</span></label>
			    <input type="text" readonly :value="type_text">
			</li>
            <li class="flex flex_a_c flex_s_b" @click="changeFlag">
                <label class="title newTitle"><span>审批状态</span></label>
                <input type="text" readonly :value="status_text">
			    <div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title newTitle"><span>审批备注</span></label>
                <textarea v-model="datails.remark"></textarea>
            </li>
        </ul>
        <!-- <ul class="looK_button_list c_flex">
            <router-link tag="li" :to="{path:'/invoiceCheck',query: {id:datails.inv_id}}" style="background-color:#3395fa;">审批</router-link>
            <router-link tag="li" :to="{path:'/invoiceConfirm',query: {id:datails.inv_id}}" style="background-color:#47a21f;">开票</router-link>
        </ul> -->
        <top-nav title="审批发票信息"  :text='"保存"' @rightClick="submit" ></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'

export default {
    name:"",
    data() {
       return {
            isIocn:[],
            addData:{},
            datails:{},
            isDatails:false,
            clientType:['未开票','已开票'],
            type:0,
            type_text:'',
            status:0,
            status_text:'',
            typeList:[  //审批类型
                {
                    key:'申请区域审批',
                    value:'1'
                },
                {
                    key:'开票区域审批',
                    value:'2'
                },
                {
                    key:'财务审批',
                    value:'3'
                },
            ],
            flagList:[  //审批状态
                {
                    key:'待审批',
                    value:'0'
                },
                {
                    key:'审批未通过',
                    value:'1'
                },
                {
                    key:'审批成功',
                    value:'2'
                },
            ]
       };
    },
    components: {},
    computed: {      
        ...mapState({
            userInfo: state => state.user.userInfo
        })    
    },
    created(){
        let {id,type} = this.$route.query
        let _this = this
        _this.ddSet.showLoad()
        _this.getInvoiceDetails({inv_id:id,type:type,managerid:_this.userInfo.id}).then(res => {
            _this.ddSet.hideLoad()
            _this.datails = res.data
            _this.isDatails = true
            if(_this.datails.type==1){
                //_this.typeList=[{key:'申请区域审批',value:'1'},{key:'开票区域审批',value:'2'}]
                
                if(_this.datails.inv_flag2==2){
                    _this.type=2
                    _this.type_text='开票区域审批'
                    _this.status=_this.datails.inv_flag2
                    _this.status_text = _this.flagList[_this.datails.inv_flag2].key
                    _this.datails.remark = _this.datails.inv_checkRemark2
                }
                else{
                    _this.type=1
                    _this.type_text='申请区域审批'
                    _this.status=_this.datails.inv_flag1
                    _this.status_text = _this.flagList[_this.datails.inv_flag1].key
                    _this.datails.remark = _this.datails.inv_checkRemark1
                }
            }
            if(_this.datails.type==2){
                //_this.typeList=[{key:'申请区域审批',value:'1'}]
                _this.type=1
                _this.type_text='申请区域审批'
                _this.status=_this.datails.inv_flag1
                _this.status_text = _this.flagList[_this.datails.inv_flag1].key
                _this.datails.remark = _this.datails.inv_checkRemark1
            }
            if(_this.datails.type==3){
                //_this.typeList=[{key:'开票区域审批',value:'2'}]
                _this.type=2
                _this.type_text='开票区域审批'
                _this.status=_this.datails.inv_flag2
                _this.status_text = _this.flagList[_this.datails.inv_flag2].key
                _this.datails.remark = _this.datails.inv_checkRemark2
            }
            if(_this.datails.type==4){
                // _this.typeList=[{key:'财务审批',value:'3'}]
                _this.type=3
                _this.type_text='财务审批'
                _this.status=_this.datails.inv_flag3
                _this.status_text = _this.flagList[_this.datails.inv_flag3].key
                _this.datails.remark = _this.datails.inv_checkRemark3
            }  
        }).catch(err => {
            _this.ddSet.hideLoad()
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getInvoiceDetails',
            'getInvoiceAudit'
        ]),
        submit(){
            let _this = this
            if(!_this.type){
                _this.ddSet.setToast({text:'请选择审批类型'})
                return
            }
            if(!_this.status){
                _this.ddSet.setToast({text:'请选择审批状态'})
                return
            }
            _this.addData.managerid = _this.userInfo.id //测试ID
            _this.addData.inv_id=_this.datails.inv_id
            _this.addData.ctype = _this.type
            _this.addData.cstatus = _this.status
            _this.addData.remark = _this.datails.remark
            _this.ddSet.showLoad()
            _this.getInvoiceAudit(_this.addData).then(res => {
                _this.ddSet.hideLoad()
                if(res.data.status){
                    _this.ddSet.setToast({text:'审核成功'}).then(res => {
                        _this.$router.go(-1)
                    })
                }else{
                    _this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })

        },
        changeType(){
            let _this = this
            let source =_this.typeList,selectedKey = _this.type_text          
            _this.ddSet.setChosen({source,selectedKey}).then(res => {
                _this.$set(_this,'type',res.value)
                _this.$set(_this,'type_text',res.key)
                if(res.value==1){
                    _this.status=_this.datails.inv_flag1
                    _this.status_text = _this.flagList[_this.datails.inv_flag1].key
                    _this.datails.remark = _this.datails.inv_checkRemark1
                }
                if(res.value==2){
                    _this.status=_this.datails.inv_flag2
                    _this.status_text = _this.flagList[_this.datails.inv_flag2].key
                    _this.datails.remark = _this.datails.inv_checkRemark2
                }
            })
        },
        changeFlag(){
            let _this = this
            let source =_this.flagList,selectedKey = _this.status_text          
            _this.ddSet.setChosen({source,selectedKey}).then(res => {
                _this.$set(_this,'status',res.value)
                _this.$set(_this,'status_text',res.key)
            })
        }
    },
}
</script>

<style scoped lang='scss'>
    .hint{
        padding: .3rem;
        font-size: .24rem;
        color: #fc0214;
        line-height: .4rem;
    }
    .looK_button_list{
        li{
            outline: none;
            background: none;
            border: none;
            width: 6.9rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
        }
    }
    .newTitle{
        font-family: SimHei;
        font-weight: bolder;
    }
</style>

