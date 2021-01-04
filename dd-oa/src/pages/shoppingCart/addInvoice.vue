<!-- 新增发票申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">{{addData.inv_oid}}</h3>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeClient">
                <label class="title"><span class="must">客户</span></label>
                <input type="text" :value="clientName" readonly placeholder="请选择客户">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">购买方名称</span></label>
                <input type="text" v-model="addData.inv_purchaserName" placeholder="请输入购买方名称">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">纳税人识别号</span></label>
                <input type="text" v-model="addData.inv_purchaserNum" placeholder="请输入纳税人识别号">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">购买方地址</span></label>
                <input type="text" v-model="addData.inv_purchaserAddress" placeholder="请输入购买方地址">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">购买方电话</span></label>
                <input type="text" v-model="addData.inv_purchaserPhone" placeholder="请输入购买方电话">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">购买方开户行</span></label>
                <input type="text" v-model="addData.inv_purchaserBank" placeholder="请输入购买方开户行">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">购买方账号</span></label>
                <input type="text" v-model="addData.inv_purchaserBankNum" placeholder="请输入购买方账号">
            </li>
            <li class="flex flex_a_c" @click="chosenServiceType">
                <label class="title"><span class="must">应税劳务</span></label>
                <input type="text" readonly v-model="addData.inv_serviceType" placeholder="请选择应税劳务">
                <div class="icon_right arrows_right"></div>
            </li>
            <li v-show="addData.inv_serviceType!='其他'" class="flex flex_a_c flex_s_b" @click="chosenServiceName(addData.inv_serviceType_text)">
                <label class="title"><span class="must">服务名称</span></label>
                <input type="text" readonly v-model="addData.inv_serviceName" placeholder="请选择服务名称">
                <div class="icon_right arrows_right"></div>
            </li>
            <li v-show="addData.inv_serviceType=='其他'" class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">服务名称</span></label>
                <input type="text" v-model="addData.inv_serviceName" placeholder="请输入服务名称">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeType">
                <label class="title newTitle"><span class="must">发票类型</span></label>
                <input type="text" readonly v-model="addData.inv_type" placeholder="请选择发票类型">
			    <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">开票金额</span></label>
                <input type="number" v-model="addData.inv_money" placeholder="请输入开票金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="chosenSentWay">
                <label class="title"><span class="must">送票方式</span></label>
                <input type="text" readonly v-model="addData.inv_sentWay" placeholder="请选择送票方式">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="chosenDArea">
                <label class="title"><span class="must">开票区域</span></label>
                <input type="text" readonly v-model="addData.inv_darea_text" placeholder="请选择开票区域">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="chosenUnit">
                <label class="title"><span class="must">开票单位</span></label>
                <input type="text" readonly v-model="addData.inv_Unit_text" placeholder="请选择开票单位">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人名称</span></label>
                <input type="text" v-model="addData.inv_receiveName" placeholder="请输入收票人名称">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人电话</span></label>
                <input type="text" v-model="addData.inv_receivePhone" placeholder="请输入收票人电话">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人地址</span></label>
                <input type="text" v-model="addData.inv_receiveAddress" placeholder="请输入收票人地址">
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="addData.inv_remark" placeholder="请输入备注"></textarea>
            </li>
        </ul>
        <!-- <input type="button" value="保存" @click="submit"> -->
        <top-nav :title='"添加发票申请信息"' :text='"保存"' @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {
	mapActions,
	mapState
} from 'vuex'
export default {
    name:"",
    data() {
       return {
            addData:{},
            then:0,
            oID:'',
            clientList:[],
            clientName:'请选择客户',
            clientId:0,
            invTypeList:[
                {
                    key:'专票',
                    value:'专票'
                },
                {
                    key:'普票',
                    value:'普票'
                },
                {
                    key:'电子发票',
                    value:'电子发票'
                }]
            // type:'',
            // paytype:0,
            // payfunction:0,
            //  files:[],
            // fileData:'',
        };
    },
    components: {},
    computed: {
        ...mapState(            
            {
            selectClientArray:state => state.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {oID,cName,cID} = this.$route.query
        this.addData.inv_oid = oID
        this.addData.inv_cid = cID
        this.clientId = cID
        this.clientName = cName
        //console.log(this.addData)
    },
    mounted() {
        //console.log("1111111"+this.selectClientArray)
        this.clientCallBack(this.selectClientArray)
    },
    methods: {
        ...mapActions([
            'getServiceType',
            'getServiceName',
            'getSentMethod',
            'getInvoiceArea',  
            'getInvoiceUnit',           
            'getInvoiceAdd'
        ]),
        submit(item){ //提交
            let _this = this
            if(_this.clientId < 1){
                _this.ddSet.setToast({text:'请先选择客户'})
				return
			}
            if(!_this.addData.inv_purchaserName){
                _this.ddSet.setToast({text:'购买方名称不能为空'})
                return
            }
            if(!_this.addData.inv_purchaserNum){
                _this.ddSet.setToast({text:'购买方纳税人识别号不能为空'})
                return
            }
            if(!_this.addData.inv_purchaserAddress){
                _this.ddSet.setToast({text:'购买方地址不能为空'})
                return
            }
            if(!_this.addData.inv_purchaserPhone){
                _this.ddSet.setToast({text:'购买方电话不能为空'})
                return
            }
            if(!_this.addData.inv_purchaserBank){
                _this.ddSet.setToast({text:'购买方开户行不能为空'})
                return
            }
            if(!_this.addData.inv_purchaserBankNum){
                _this.ddSet.setToast({text:'购买方账号不能为空'})
                return
            }
            if(!_this.addData.inv_serviceType){
                _this.ddSet.setToast({text:'请您选择应税劳务'})
                return
            }
            if(!_this.addData.inv_serviceName){
                _this.ddSet.setToast({text:'请您选择服务名称'})
                return
            }
            if(!_this.addData.inv_type){
                _this.ddSet.setToast({text:'请您选择发票类型'})
                return
            }
            if(!_this.addData.inv_money){
                _this.ddSet.setToast({text:'开票金额不能为空'})
                return
            }
            if(!_this.addData.inv_sentWay){
                _this.ddSet.setToast({text:'请您选择送票方式'})
                return
            }
            if(!_this.addData.inv_darea){
                _this.ddSet.setToast({text:'请您选择开票区域'})
                return
            }
            if(!_this.addData.inv_Unit){
                _this.ddSet.setToast({text:'请您选择开票单位'})
                return
            }
            this.addData.uba_oid = this.oID
            _this.addData.managerid = _this.userInfo.id //测试ID            
            console.log(_this.addData)
            _this.ddSet.showLoad()
            _this.getInvoiceAdd(_this.addData).then(res => {
                if(res.data.status){                    
                    _this.ddSet.setToast({text:'新增发票申请信息成功'}).then(res => {
                        _this.ddSet.hideLoad()
                        _this.$router.go(-1)
                    })
                }else{
                    _this.ddSet.setToast({text:res.data.msg})
                    _this.ddSet.hideLoad()
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })
        },
        changeClient(){ //选择客户
    		let _this = this;
    		_this.$router.push({ path: '/addOrders/customerSelect', query: { selected_id: _this.clientId }})
        },
    	clientCallBack(_selectData){
    		if(_selectData.length){
    			this.clientName = _selectData[0].name;
    			this.clientId = _selectData[0].id;
    			// this.$set(this.addData,'co_name',_selectData[0].co_name)
    			// this.$set(this.addData,'co_number',_selectData[0].co_number)
    			this.$set(this.addData,'inv_cid',_selectData[0].id)
    		}
    		// else{
            //     console.log(2222222222)
    		// 	// this.clientName = '请选择客户';
    		// 	// this.clientId = 0;
    		// 	// this.$set(this.addData,'co_name','')
    		// 	// this.$set(this.addData,'co_number','')
    		// 	this.$set(this.addData,'inv_cid',0)
    		// }
        },
        chosenServiceType(){
            let _this = this
            this.getServiceType({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.inv_serviceType
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'inv_serviceType',res.key)
                    _this.$set(_this.addData,'inv_serviceType_text',res.value)
                    _this.$set(_this.addData,'inv_serviceName',"")
                    _this.$set(_this.addData,'inv_serviceName_text',"")
                })
            })
        },
        chosenServiceName(_serviceType){
            let _this = this
            this.getServiceName({inv_serviceType:_serviceType}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.inv_serviceName
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'inv_serviceName',res.key)
                    _this.$set(_this.addData,'inv_serviceName_text',res.value)
                })
            })
        },
        chosenSentWay(){
            let _this = this
            this.getSentMethod({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.inv_sentWay
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'inv_sentWay',res.key)
                    _this.$set(_this.addData,'inv_sentWay_text',res.value)
                })
            })
        },
        chosenDArea(){
            let _this = this
            _this.getInvoiceArea({ddkey:'dingzreafyvgzklylomj'}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.inv_darea
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'inv_darea',res.value)
                    _this.$set(_this.addData,'inv_darea_text',res.key)
                })
            })
        },
        chosenUnit(){
            let _this = this
            if(!_this.addData.inv_darea){
                _this.ddSet.setToast({text:'请您选择开票区域'})
                return
            }
            _this.getInvoiceUnit({area:_this.addData.inv_darea,ddkey:'dingzreafyvgzklylomj'}).then(res => {
                let source = []
                let selectedKey = _this.addData.inv_Unit
                res.data.list.map((item,index) => {
                    let obj = {
                        key:item.invU_name,
                        value:item.invU_id
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'inv_Unit',res.value)
                    _this.$set(_this.addData,'inv_Unit_text',res.key)
                })
            })
        },
        changeType(){
            let _this = this
            let source =_this.invTypeList,selectedKey = _this.addData.inv_type          
            _this.ddSet.setChosen({source,selectedKey}).then(res => {
                _this.$set(_this.addData,'inv_type',res.key)
                _this.$set(_this.addData,'inv_type_text',res.value)
            })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang='scss'>

</style>
