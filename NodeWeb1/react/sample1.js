
//1. hello world
ReactDOM.render(<h1>Hello, world!</h1>, document.getElementById('example1'));

//2. Components
class ShoppingList extends React.Component {
    render() {
        return (
            <div className="shopping-list">
                <h1>Shopping List for {this.props.name}</h1>
                <ul>
                    <li>Instagram</li>
                    <li>WhatsApp</li>
                    <li>Oculus</li>
                </ul>
            </div>
        );
    }
}
ReactDOM.render(<ShoppingList name='Hellen' />, document.getElementById('example2'))

//3. States
var ToggleButton = React.createClass({
    getInitialState: function () {
        return { liked: this.props.toggle, count: 0 }
    },
    handleClick: function () {
        this.setState({ liked: !this.state.liked, count: this.state.count + 1 })
    },
    render: function () {
        var text = this.state.liked ? 'OPEN' : 'CLOSE'
        return (
            <div onClick={this.handleClick}>
                <div>Box is <strong>{text}</strong>, u have click count: <strong>{this.state.count}</strong>  </div>
            </div>
        )
    }
})
ReactDOM.render(<ToggleButton toggle='true' />, document.getElementById('example3'))

//4. Bindable changing
var SyncText = React.createClass({
    getInitialState: function () {
        return { value: 'value,' }
    },
    handleChange: function (event) {
        this.setState({ value: event.target.value });
    },
    render: function () {
        var value = this.state.value
        return (
            <div>
                <br />
                <input value={value} onChange={this.handleChange} />
                <h3> {value} </h3>
            </div>
        )
    }
})
ReactDOM.render(<SyncText />, document.getElementById('example4'))

//5. Clock 
var ExampleApplication = React.createClass({
    render: function () {
        var elapsed = Math.round(this.props.elapsed / 100);
        var seconds = elapsed / 10 + (elapsed % 10 ? '' : '.0');
        var message =
            'React has been successfully running for ' + seconds + ' seconds.';

        return React.DOM.p(null, message);
    }
});

// Call React.createFactory instead of directly call ExampleApplication({...}) in React.render
var ExampleApplicationFactory = React.createFactory(ExampleApplication);

var start = new Date().getTime();
setInterval(function () {
    ReactDOM.render(
        //ExampleApplicationFactory({ elapsed: new Date().getTime() - start }),
        <ExampleApplication elapsed={new Date().getTime() - start} />,
        document.getElementById('example5')
    );
}, 50);


//6. Calculator

var QuadraticCalculator = React.createClass({
  getInitialState: function() {
    return {
      a: 1,
      b: 3,
      c: -4
    };
  },

  /**
   * This function will be re-bound in render multiple times. Each .bind() will
   * create a new function that calls this with the appropriate key as well as
   * the event. The key is the key in the state object that the value should be
   * mapped from.
   */
  handleInputChange: function(key, event) {
    var partialState = {};
    partialState[key] = parseFloat(event.target.value);
    this.setState(partialState);
  },

  render: function() {
    var a = this.state.a;
    var b = this.state.b;
    var c = this.state.c;
    var root = Math.sqrt(Math.pow(b, 2) - 4 * a * c);
    var denominator = 2 * a;
    var x1 = (-b + root) / denominator;
    var x2 = (-b - root) / denominator;
    return (
      <div>
        <strong>
          <em>ax</em><sup>2</sup> + <em>bx</em> + <em>c</em> = 0
        </strong>
        <h4>Solve for <em>x</em>:</h4>
        <p>
          <label>
            a: <input type="number" value={a} onChange={this.handleInputChange.bind(null, 'a')} />
          </label>
          <br />
          <label>
            b: <input type="number" value={b} onChange={this.handleInputChange.bind(null, 'b')} />
          </label>
          <br />
          <label>
            c: <input type="number" value={c} onChange={this.handleInputChange.bind(null, 'c')} />
          </label>
          <br />
          x: <strong>{x1}, {x2}</strong>
        </p>
      </div>
    );
  }
});

ReactDOM.render(
  <QuadraticCalculator />,
  document.getElementById('example6')
);





