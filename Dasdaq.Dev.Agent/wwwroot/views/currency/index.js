﻿component.data = function () {
    return {
        currency: {
            name: null,
            amount: "1000000000.0000",
            account: "eosio.token"
        },
        currencies: [],
        accounts: [],
        currencyView: null
    };
};

component.created = function () {
    var self = this;
    self.currencyView = qv.createView('/api/eos/currency', {}, 5 * 1000);
    self.currencyView.fetch(x => {
        self.currencies = x.data;
    });
    qv.createView('/api/eos/account', {}, 5 * 1000).fetch(x => {
        self.accounts = x.data;
    });
};

component.methods = {
    supply: function (currency) {
        var account = $('#currency-' + currency).find('select').val();
        var amount = $('#currency-' + currency).find('input[type="text"]').val();
        app.notification("pending", `正在为${account}发放${amount} ${currency}...`);
        qv.post('/api/eos/currency/' + currency + '/account/' + account, { amount: amount })
            .then(x => {
                app.notification("succeeded", `${currency}已经成功发放`);
            })
            .catch(err => {
                app.notification("error", currency + "发放失败", err.responseJSON.msg);
            });
    },
    newCurrency: function () {
        var self = this;
        app.notification("pending", `正在创建${self.currency.amount} ${self.currency.name}...`);
        qv.put('/api/eos/currency/' + self.currency.name, {
            account: self.currency.account,
            amount: self.currency.amount
        })
            .then(x => {
                self.currencyView.refresh();
                app.notification("succeeded", `${self.currency.amount} ${self.currency.name}已经创建成功`);
                self.currency.name = null;
                self.currency.amount = "1000000000.0000";
                self.currency.account = "eosio.token";
            })
            .catch(err => {
                app.notification("error", `${self.currency.amount} ${self.currency.name}创建失败`, err.responseJSON.msg);
            });
    }
};