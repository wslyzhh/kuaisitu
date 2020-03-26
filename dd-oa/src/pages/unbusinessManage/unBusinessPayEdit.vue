<!-- 编辑非业务支付申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="li_auto flex" v-show="editData.uba_oid">
                <label class="title"><span>订单号</span></label>
                <router-link tag="h3" class="hint_1" :to="{path:'/modifyOrder', query: { id: editData.uba_oid }}">{{editData.uba_oid}}</router-link>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span>支付类别</span></label>
                <input type="text" readonly v-model="editData.uba_typeText">
            </li>
            <li class="flex li_auto" v-if="editData.uba_type>0 ">
                <label class="title"><span class="must">支付用途</span></label>
                <textarea v-model="editData.uba_function" placeholder="请输入支付用途内容"></textarea>
            </li>
            <li class="flex flex_a_c" @click="chosenPayFunction" v-else>
                <label class="title"><span class="must">支付用途</span></label>
                <input type="text" readonly v-model="editData.uba_function" placeholder="请选择支付用途">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex li_auto">
                <label class="title"><span>用途说明</span></label>
                <textarea v-model="editData.uba_instruction" placeholder="请输入用途说明内容"></textarea>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款银行</span></label>
                <input type="text" v-model="editData.uba_receiveBank" placeholder="请输入收款银行">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>账户名称</span></label>
                <input type="text" v-model="editData.uba_receiveBankName" placeholder="请输入账户名称">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款账号</span></label>
                <input type="text" v-model="editData.uba_receiveBankNum" placeholder="请输入收款账号">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">金额</span></label>
                <input type="text" v-model="editData.uba_money" placeholder="请输入有效金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectDate">
                <label class="title"><span class="must">预付日期</span></label>
                <input type="text" readonly :value="getListDate(editData.uba_foreDate)" placeholder="请选择日期"  >
                <div class="icon_right time"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="editData.uba_remark" placeholder="请输入备注"></textarea>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>附件</span></label>
				<div class="icon_right accessory">
					<input type="file" multiple="multiple" ref="FileUp" @change="upFile($event)">
				</div>
            </li>
			<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files" :key="index">
				<a :href="'/' + f.pp_filePath">{{f.pp_fileName}}</a>
				<span>{{f.pp_size}}K</span>
			    <div class="icon_right delete"	@click="delPFile(f.pp_id,f.pp_fileName)"></div>
			</li>
        </ul>
        <top-nav :title='type == "add" ? "添加非业务支付信息":"编辑非业务支付信息"' :text='"保存"' @rightClick="submit"></top-nav>
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
            editData:{},
            then:0,
            type:'',
            paytype:0,
            payfunction:0,
            files:[],
            fileData:'',
            ubaid:0
            //oID:'',
        };
    },
    components: {},
    computed: {
        ...mapState(            
            {
            userInfo: state => state.user.userInfo
        }) 
     },
    created(){
        let _this = this
        let {uba_id,type} = _this.$route.query        
        _this.type = type
        _this.ubaid = uba_id
        // this.paytype = paytype
        // this.payfunction = payfunction
        // this.editData.uba_oid = oID
        if(type == 'EDIT'){
            let params = {
                uba_id           
            }
            _this.getUnBusinessPayDetails(params).then(res => {
                _this.editData = res.data
                if(_this.editData.uba_type == 0) {
                   if(_this.editData.uba_function == "业务活动执行备用金借款") {
                        _this.paytype = 0
                        _this.payfunction = 0
                   } else{                       
                        _this.paytype = 1
                        _this.payfunction = 1
                   }
                }                
                _this.files = _this.editData.picList
                _this.editData.uba_foreDate = _this.editData.uba_foreDate
                console.log(_this.files)
            })
        }
    },
    mounted() {
        //this.editData.uba_foreDate = formatDate(this.editData.uba_foreDate, 'yyyy-MM-dd')
    },
    methods: {
        ...mapActions([
            'getUnBusinessNature',
            'getUnBusinessPayFunction',
            'getUnBusinessPayDetails',
            'getUnBusinessPayEdit',
            'getUpLoadFile',
            'delFile'
        ]),
        upFile(e){
            let _this = this
            _this.fileData = _this.$refs.FileUp.files
            Object.keys(_this.fileData).map((item,index)=>{
                _this['files'].push({
                    pp_fileName:_this.fileData[index].name,
                    pp_id:0
                })
            })     
        },
		delPFile(_fid,_fileName){
            let _this = this;
            _this.ddSet.setConfirm('确定要删除《'+_fileName+'》文件吗？').then(res=>{
                if(0 == res.buttonIndex){
                    _this.ddSet.showLoad()
                    if(_fid){
                        _this.delFile({
					   	fileID:_fid,
					   	type:2,
					   	managerid:_this.userInfo.id
                        }).then(res => {
                            _this.ddSet.hideLoad()
                            if(1 == res.data.status){
                                _this.ddSet.setToast({text:'删除文件成功'})
                                _this['files'] = _this['files'].filter(function(item){
                                    return item.pp_id != _fid
                                })
                            }
                            else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                        }).catch(err => {
                            _this.ddSet.hideLoad()
                        })
                    }
                    else{
                        _this.ddSet.hideLoad()
                        _this['files'] = _this['files'].filter(function(item){
                            return item.pp_id != _fid
                        })
                    }
                }
            }) 			
		},
        submit(item){ //提交
            let _this = this
            if(!_this.editData.uba_function){
                _this.ddSet.setToast({text:'支付用途不能为空'})
                return
            }
            if(!_this.editData.uba_money){
                _this.ddSet.setToast({text:'请填写金额'})
                return
            }
            if(!_this.editData.uba_foreDate){
                _this.ddSet.setToast({text:'预付日期不能为空'})
                return
            }
            _this.editData.managerid = _this.userInfo.id //测试ID
            _this.ddSet.showLoad()
            _this.getUnBusinessPayEdit(_this.editData).then(res => {
                if(res.data.status){
                    //上传附件                                      
                    if(_this.fileData.length > 0){
                        var data = new FormData();
                        Object.keys(_this.fileData).map((item,index)=>{
                            data.append("file",_this.fileData[index])                
                        }) 
                        data.append("type",2)
                        data.append("keyID",_this.ubaid)
                        data.append("fileType",2)
                        data.append("managerid",_this.userInfo.id)
                        _this.getUpLoadFile(data).then(res => {
                            _this.ddSet.hideLoad()
                            if(res.data.status){
                                _this.ddSet.setToast({text:'编辑信息成功'}).then(res => {
                                    _this.$router.go(-1)
                                })
                            }
                            else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                        })
                    }
                    else{
                        _this.ddSet.hideLoad()
                        _this.ddSet.setToast({text:'编辑信息成功'}).then(res => {
                            _this.$router.go(-1)
                        })
                    }
                }else{
                    this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                this.ddSet.hideLoad()
            })
        },
        selectDate(){ //活动日期
			let _this = this
			_this.ddSet.setDatepicker().then(res => {
                _this.$set(this.editData,'uba_foreDate',res.value)
            })
        },                    
        chosenPayType(){   //支付类别
            let _this = this
            this.getUnBusinessNature({type:this.paytype}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.editData.uba_type
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.editData,'uba_type',res.value)
                    _this.$set(_this.editData,'uba_type_text',res.key)
                })
            })
        },                 
        chosenPayFunction(){   //支付用途
            let _this = this
            console.log(this.payfunction)
            this.getUnBusinessPayFunction({type:this.payfunction}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.editData.uba_function
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.editData,'uba_function',res.value)
                    _this.$set(_this.editData,'uba_function_text',res.key)
                })
            })
        },
		// 处理 时间
		getListDate(_date){
			if(!_date){
				return '';
			}
			let tmp = _date.split('T');
			return tmp[0]
		}
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>

