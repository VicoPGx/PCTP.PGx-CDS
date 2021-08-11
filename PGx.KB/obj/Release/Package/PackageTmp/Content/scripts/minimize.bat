:: Launch Microsoft Ajax Minifier Command Prompt, and navigate to this folder. Then, run this bat.

:: Minimize js files

ajaxmin -clobber:true controls.js -o controls.min.js

ajaxmin -clobber:true utility.edit.plan.js -o utility.edit.plan.min.js

ajaxmin -clobber:true utility.edit.rule.js -o utility.edit.rule.min.js

ajaxmin -clobber:true utility.js -o utility.min.js

ajaxmin -clobber:true objectDiff.js -o objectDiff.min.js

:: Combine js files (exclued jquery.multiselect.zh-cn.min.js for English-versioned mutliselect)

del combined.min.js

copy /b /y jquery-1.8.3.min.js + line + jquery-ui.min.js + line + jquery.dataTables.min.js + line + jquery.dataTables.custom.min.js + line +  controls.min.js + jquery-ui-timepicker-addon.min.js + line + jquery.dd.min.js + line + jquery.editable-select.min.js + line + jquery.ezmark.min.js + line + jquery.form.min.js + line + jquery.multiselect.min.js + line + jquery.multiselect.filter.min.js + line + jquery.numeric.min.js + line + jquery.tablednd.min.js + line + jquery.ui.spinner.min.js + line + objectDiff.min.js + line + utility.edit.plan.min.js + line + utility.edit.rule.min.js + line + utility.min.js + line + jquery.multiselect.zh-cn.min.js + line combined.min.js 

copy /b /y jquery-1.8.3.min.js + line + jquery-ui.min.js + line + jquery.dataTables.min.js + line + jquery.dataTables.custom.min.js + line +  utility.min.js + line combined.lite.min.js 

:: Minimize combined file

:: ajaxmin -clobber:true combined.min.js -o combined.min.js