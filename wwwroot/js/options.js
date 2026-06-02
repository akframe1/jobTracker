async function calculateBinomial() {
    const spotPrice    = parseFloat(document.getElementById('b-spot-price').value);
    const strikePrice  = parseFloat(document.getElementById('b-strike-price').value);
    const timeToExpiry = parseFloat(document.getElementById('b-time-expiry').value);
    const riskFreeRate = parseFloat(document.getElementById('b-risk-free-rate').value);
    const volatility   = parseFloat(document.getElementById('b-volatility').value);
    const steps        = parseInt(document.getElementById('b-steps').value);

    if ([spotPrice, strikePrice, timeToExpiry, riskFreeRate, volatility, steps].some(isNaN)) {
        alert('Please fill in all fields.');
        return;
    }

    const btn = document.querySelector('.options-btn');
    btn.disabled = true;
    btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Calculating...';

    try {
        const res = await fetch('/options/binomial', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ spotPrice, strikePrice, timeToExpiry, riskFreeRate, volatility, steps })
        });

        if (!res.ok) {
            const err = await res.json();
            alert(err.message);
            return;
        }

        const data = await res.json();

        document.getElementById('b-american-call').textContent = data.americanCallPrice;
        document.getElementById('b-american-put').textContent  = data.americanPutPrice;
        document.getElementById('b-european-call').textContent = data.europeanCallPrice;
        document.getElementById('b-european-put').textContent  = data.europeanPutPrice;
        document.getElementById('b-premium-call').textContent  = data.earlyExercisePremiumCall;
        document.getElementById('b-premium-put').textContent   = data.earlyExercisePremiumPut;
        document.getElementById('b-up-factor').textContent     = data.upFactor;
        document.getElementById('b-down-factor').textContent   = data.downFactor;
        document.getElementById('b-rn-prob').textContent       = data.riskNeutralProbability;
        document.getElementById('b-dt').textContent            = data.timeStepSize;
        document.getElementById('b-call-delta').textContent    = data.callDelta;
        document.getElementById('b-put-delta').textContent     = data.putDelta;

        document.getElementById('binomial-result').style.display = 'block';

    } catch (err) {
        alert('Something went wrong. Please try again.');
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="fa-solid fa-calculator"></i> Calculate';
    }
}